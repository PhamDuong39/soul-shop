﻿using Microsoft.AspNetCore.Http;
using Soul.Shop.Module.ApiProfiler.Internal;

namespace Soul.Shop.Module.ApiProfiler;

/// <summary>
/// Options for configuring MiniProfiler
/// </summary>
public class MiniProfilerOptions : MiniProfilerBaseOptions
{
    /// <summary>
    /// The path under which ALL routes are registered in, defaults to the application root.  For example, "/myDirectory/" would yield
    /// "/myDirectory/includes.min.js" rather than "/mini-profiler-resources/includes.min.js"
    /// Any setting here should be absolute for the application, e.g. "/myDirectory/"
    /// /mini-profiler-resources
    /// </summary>
    public PathString RouteBasePath { get; set; } = "/profiler";

    /// <summary>
    /// Set a function to control whether a given request should be profiled at all.
    /// </summary>
    public Func<HttpRequest, bool> ShouldProfile { get; set; } = _ => true;

    /// <summary>
    /// A function that determines who can access the MiniProfiler results URL and list URL.  It should return true when
    /// the request client has access to results, false for a 401 to be returned. HttpRequest parameter is the current request and
    /// </summary>
    /// <remarks>
    /// The HttpRequest parameter that will be passed into this function should never be null.
    /// </remarks>
    public Func<HttpRequest, bool> ResultsAuthorize { get; set; }

    /// <summary>
    /// Special authorization function that is called for the list results (listing all the profiling sessions), 
    /// we also test for results authorize always. This must be set and return true, to enable the listing feature.
    /// </summary>
    public Func<HttpRequest, bool> ResultsListAuthorize { get; set; }

    /// <summary>
    /// Function to provide the unique user ID based on the request, to store MiniProfiler IDs user
    /// </summary>
    public Func<HttpRequest, string> UserIdProvider { get; set; } =
        request => request.HttpContext.Connection.RemoteIpAddress?.ToString();
}