﻿using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Soul.Shop.Module.ApiProfiler.Internal;

namespace Soul.Shop.Module.ApiProfiler;

/// <summary>
/// Represents a middleware that starts and stops a MiniProfiler
/// </summary>
public class MiniProfilerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostingEnvironment _env;
    private readonly IOptions<MiniProfilerOptions> _options;

    internal readonly EmbeddedProvider Embedded;

    internal MiniProfilerOptions Options => _options.Value;

    /// <summary>
    /// Creates a new instance of <see cref="MiniProfilerMiddleware"/>
    /// </summary>
    /// <param name="next">The delegate representing the next middleware in the request pipeline.</param>
    /// <param name="hostingEnvironment">The Hosting Environment.</param>
    /// <param name="options">The middleware options, containing the rules to apply.</param>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="next"/>, <paramref name="hostingEnvironment"/>, or <paramref name="options"/> is <c>null</c>.</exception>
    public MiniProfilerMiddleware(RequestDelegate next, IHostingEnvironment hostingEnvironment,
        IOptions<MiniProfilerOptions> options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _env = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        Embedded = new EmbeddedProvider(_options, _env);
    }

    /// <summary>
    /// Executes the MiniProfiler-wrapped middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task that represents the execution of the MiniProfiler-wrapped middleware.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="context"/> is <c>null</c>.</exception>
    public async Task Invoke(HttpContext context)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));

        if (context.Request.Path.StartsWithSegments(Options.RouteBasePath, out var subPath))
        {
            // This is a request in the MiniProfiler path (e.g. one of "our" routes), HANDLE THE SITUATION.
            await HandleRequest(context, subPath).ConfigureAwait(false);
            return;
        }

        // Otherwise this is an app request, profile it!
        if (ShouldProfile(context.Request))
        {
            // Wrap the request in this profiler
            var mp = Options.StartProfiler();

            // Set the user
            mp.User = Options.UserIdProvider?.Invoke(context.Request);

            // Always add this profiler's header (and any async requests before it)
            using (mp.StepIf("MiniProfiler Prep", 0.1m))
            {
                await SetHeadersAndState(context, mp).ConfigureAwait(false);
            }
            // Execute the pipe
#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
            await _next(context);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.
            // Assign name
            EnsureName(mp, context);
            // Stop (and record)
            await mp.StopAsync().ConfigureAwait(false);
        }
        else
        {
            // Don't profile, only relay
#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
            await _next(context);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.
        }
    }

    private bool ShouldProfile(HttpRequest request)
    {
        foreach (var ignored in Options.IgnoredPaths)
            if (ignored != null && request.Path.Value.Contains(ignored, StringComparison.OrdinalIgnoreCase))
                return false;
        return Options.ShouldProfile?.Invoke(request) ?? true;
    }

    private void EnsureName(MiniProfiler profiler, HttpContext context)
    {
        if (profiler.Name == nameof(MiniProfiler))
        {
            var url = StringBuilderCache.Get()
                .Append(context.Request.Scheme)
                .Append("://")
                .Append(context.Request.Host.Value)
                .Append(context.Request.PathBase.Value)
                .Append(context.Request.Path.Value)
                .Append(context.Request.QueryString.Value)
                .ToStringRecycle();

            var routeData = (context.Features[typeof(IRoutingFeature)] as IRoutingFeature)?.RouteData;
            if (routeData != null)
            {
                profiler.Name = routeData.Values["controller"] + "/" + routeData.Values["action"];
            }
            else
            {
                profiler.Name = url;
                if (profiler.Name.Length > 50)
                    profiler.Name = profiler.Name.Remove(50);
            }

            if (profiler.Root?.Name == nameof(MiniProfiler)) profiler.Root.Name = url;
        }
    }

    private async Task SetHeadersAndState(HttpContext context, MiniProfiler current)
    {
        try
        {
            // Are we authorized???
            var isAuthorized = Options.ResultsAuthorize?.Invoke(context.Request) ?? true;

            // Grab any past profilers (e.g. from a previous redirect)
            var profilerIds =
                (isAuthorized ? await Options.ExpireAndGetUnviewedAsync(current.User).ConfigureAwait(false) : null)
                ?? new List<Guid>(1);

            // Always add the current
            profilerIds.Add(current.Id);

            if (profilerIds.Count > 0) context.Response.Headers.Add("X-MiniProfiler-Ids", profilerIds.ToJson());

            // Set the state to use in RenderIncludes() down the pipe later
            new RequestState { IsAuthorized = isAuthorized, RequestIDs = profilerIds }.Store(context);
        }
        catch
        {
            /* oh no! headers blew up */
        }
    }

    private async Task HandleRequest(HttpContext context, PathString subPath)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        string result = null;

        // File embed
        if (subPath.Value.StartsWith("/includes.min", StringComparison.Ordinal))
            result = Embedded.GetFile(context, subPath);

        switch (subPath.Value)
        {
            case "/results-index":
                result = ResultsIndex(context);
                break;

            case "/results-list":
                result = await ResultsListAsync(context).ConfigureAwait(false);
                break;

            case "/results-stat":
                result = ResultsStatIndex(context);
                break;

            case "/results-stat-list":
                result = await ResultsStatListAsync(context).ConfigureAwait(false);
                break;

            case "/results":
                result = await GetSingleProfilerResultAsync(context).ConfigureAwait(false);
                break;
        }

        result ??= NotFound(context, "Not Found: " + subPath);
        context.Response.ContentLength = result != null ? Encoding.UTF8.GetByteCount(result) : 0;

        await context.Response.WriteAsync(result).ConfigureAwait(false);
    }

    private static string NotFound(HttpContext context, string message = null, string contentType = "text/plain")
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = contentType;

        return message;
    }

    /// <summary>
    /// Returns true if the current request is allowed to see the profiler response.
    /// </summary>
    /// <param name="context">The context to attempt to authorize a user for.</param>
    /// <param name="isList">Whether this is a list route being accessed.</param>
    /// <param name="message">The access denied message, if present.</param>
    private bool AuthorizeRequest(HttpContext context, bool isList, out string message)
    {
        message = null;
        var authorize = Options.ResultsAuthorize;
        var authorizeList = Options.ResultsListAuthorize;

        if ((authorize != null && !authorize(context.Request)) ||
            (isList && authorizeList != null && !authorizeList(context.Request)))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "text/plain";
            message = "unauthorized";
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns the list of profiling sessions
    /// </summary>
    /// <param name="context">The results list HTML, if authorized.</param>
    private string ResultsIndex(HttpContext context)
    {
        if (!AuthorizeRequest(context, true, out var message)) return message;

        context.Response.ContentType = "text/html; charset=utf-8";

        var path = context.Request.PathBase + Options.RouteBasePath.Value.EnsureTrailingSlash();
        return Render.ResultListHtml(Options, path,
            context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty);
    }

    /// <summary>
    /// 统计首页
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private string ResultsStatIndex(HttpContext context)
    {
        if (!AuthorizeRequest(context, true, out var message)) return message;

        context.Response.ContentType = "text/html; charset=utf-8";

        var path = context.Request.PathBase + Options.RouteBasePath.Value.EnsureTrailingSlash();
        return Render.ResultStatListHtml(Options, path,
            context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty);
    }

    /// <summary>
    /// Returns the JSON needed for the results list in MiniProfiler
    /// </summary>
    /// <param name="context">The context to get the results list for.</param>
    private async Task<string> ResultsListAsync(HttpContext context)
    {
        if (!AuthorizeRequest(context, true, out var message)) return message;

        string take = context.Request.Query["take"];
        var top = 100;
        if (!string.IsNullOrWhiteSpace(take) && int.TryParse(take, out var t) && t > 0) top = t;
        var guids = await Options.Storage.ListAsync(top).ConfigureAwait(false);

        if (context.Request.Query.TryGetValue("last-id", out var lastId) && Guid.TryParse(lastId, out var lastGuid))
            guids = guids.TakeWhile(g => g != lastGuid);

        return guids.Reverse()
            .Select(g => Options.Storage.Load(g))
            .Where(p => p != null)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.ClientTimings,
                Started = p.Started.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                p.HasUserViewed,
                p.MachineName,
                p.User,
                p.DurationMilliseconds
            }).ToJson();
    }

    /// <summary>
    /// 统计列表
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<string> ResultsStatListAsync(HttpContext context)
    {
        if (!AuthorizeRequest(context, true, out var message)) return message;

        string take = context.Request.Query["take"];
        var top = 1000;
        if (!string.IsNullOrWhiteSpace(take) && int.TryParse(take, out var t) && t > 0) top = t;
        var guids = await Options.Storage.ListAsync(top).ConfigureAwait(false);

        return guids.Reverse()
            .Select(g => Options.Storage.Load(g))
            .Where(p => p != null)
            .GroupBy(g => g.Name)
            .Select(p => new
            {
                Name = p.Key,
                TotalCount = p.Count(),
                MaxDurationMilliseconds = p.Max(x => x.DurationMilliseconds),
                MinDurationMilliseconds = p.Min(x => x.DurationMilliseconds),
                AvgDurationMilliseconds = Math.Round(p.Sum(x => x.DurationMilliseconds) / p.Count(), 2)
            }).OrderBy(c => c.Name).ToJson();
    }

    /// <summary>
    /// Returns either JSON or full page HTML of a previous <c>MiniProfiler</c> session, 
    /// identified by its <c>"?id=GUID"</c> on the query.
    /// </summary>
    /// <param name="context">The context to get a profiler response for.</param>
    private async Task<string> GetSingleProfilerResultAsync(HttpContext context)
    {
        Guid id;
        ResultRequest clientRequest = null;
        // When we're rendering as a button/popup in the corner, it's an AJAX/JSON request.
        // If that's absent, we're rendering results as a full page for sharing.
        var jsonRequest = context.Request.Headers["Accept"].FirstOrDefault()?.Contains("application/json") == true;

        // Try to parse from the JSON payload first
        if (jsonRequest
            && context.Request.ContentLength > 0
            && ResultRequest.TryParse(context.Request.Body, out clientRequest)
            && clientRequest.Id.HasValue)
        {
            id = clientRequest.Id.Value;
        }
        else if (Guid.TryParse(context.Request.Query["id"], out id))
        {
            // We got the guid from the querystring
        }
        else if (Options.StopwatchProvider != null)
        {
            // Fall back to the last result
            id = (await Options.Storage.ListAsync(1).ConfigureAwait(false)).FirstOrDefault();
        }

        if (id == default) return NotFound(context, jsonRequest ? null : "No GUID id specified on the query string");

        var profiler = await Options.Storage.LoadAsync(id).ConfigureAwait(false);
        var user = Options.UserIdProvider?.Invoke(context.Request);

        await Options.Storage.SetViewedAsync(user, id).ConfigureAwait(false);

        if (profiler == null)
            return NotFound(context, jsonRequest ? null : "No MiniProfiler results found with Id=" + id.ToString());

        var needsSave = false;
        if (profiler.ClientTimings == null && clientRequest?.TimingCount > 0)
        {
            profiler.ClientTimings = ClientTimings.FromRequest(clientRequest);
            needsSave = true;
        }

        if (!profiler.HasUserViewed)
        {
            profiler.HasUserViewed = true;
            needsSave = true;
        }

        if (needsSave) await Options.Storage.SaveAsync(profiler).ConfigureAwait(false);

        if (!AuthorizeRequest(context, false, out var authorizeMessage))
        {
            context.Response.ContentType = "application/json";
            return @"""hidden"""; // JSON
        }

        if (jsonRequest)
        {
            context.Response.ContentType = "application/json";
            return profiler.ToJson();
        }
        else
        {
            context.Response.ContentType = "text/html; charset=utf-8";
            return Render.SingleResultHtml(profiler,
                context.Request.PathBase + Options.RouteBasePath.Value.EnsureTrailingSlash());
        }
    }
}