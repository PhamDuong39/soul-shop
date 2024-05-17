using System.Net;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Soul.Shop.Module.Auth.Dashboard;

public class AspNetCoreDashboardMiddleware(RequestDelegate next, DashboardOptions options)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly DashboardOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public async Task Invoke(HttpContext httpContext)
    {
        var context = new AspNetCoreDashboardContext(_options, httpContext);

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var filter in _options.Authorization)
            if (!filter.Authorize(context))
            {
                var isAuthenticated = httpContext.User?.Identity?.IsAuthenticated;

                httpContext.Response.StatusCode = isAuthenticated == true
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
                return;
            }

        if (!_options.IgnoreAntiforgeryToken)
        {
            var antiforgery = httpContext.RequestServices.GetService<IAntiforgery>();

            if (antiforgery != null)
            {
                var requestValid = await antiforgery.IsRequestValidAsync(httpContext);

                if (!requestValid)
                {
                    // Invalid or missing CSRF token
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }
        }

        await _next.Invoke(httpContext);
    }
}