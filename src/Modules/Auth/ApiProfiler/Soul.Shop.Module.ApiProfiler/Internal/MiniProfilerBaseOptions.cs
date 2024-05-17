using System.Reflection;
using Soul.Shop.Module.ApiProfiler.Helpers;
using Soul.Shop.Module.ApiProfiler.ProfileProviders;
using Soul.Shop.Module.ApiProfiler.Storage;

namespace Soul.Shop.Module.ApiProfiler.Internal;

/// <summary>
/// Various configuration properties for MiniProfiler.
/// </summary>
public class MiniProfilerBaseOptions
{
    /// <summary>
    /// Assembly version of this dank MiniProfiler.
    /// </summary>
    public static Version Version { get; } = typeof(MiniProfilerBaseOptions).GetTypeInfo().Assembly.GetName().Version;

    /// <summary>
    /// The hash to use for file cache breaking, this is automatically calculated.
    /// </summary>
    public virtual string VersionHash { get; set; } =
        typeof(MiniProfilerBaseOptions).GetTypeInfo().Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? Version.ToString();

    /// <summary>
    /// When <see cref="IAsyncProfilerProvider.Start(string, MiniProfilerBaseOptions)"/> is called, if the current request URL contains any items in this property,
    /// no profiler will be instantiated and no results will be displayed.
    /// Default value is { "/content/", "/scripts/", "/favicon.ico" }.
    /// </summary>
    public HashSet<string> IgnoredPaths { get; } =
    [
        "/content/",
        "/scripts/",
        "/favicon.ico"
    ];

    /// <summary>
    /// The maximum number of unviewed profiler sessions (set this low cause we don't want to blow up headers)
    /// </summary>
    public int MaxUnviewedProfiles { get; set; } = 20;

    /// <summary>
    /// Any Timing step with a duration less than or equal to this will be hidden by default in the UI; defaults to 2.0 ms.
    /// </summary>
    public decimal TrivialDurationThresholdMilliseconds { get; set; } = 2.0M;

    /// <summary>
    /// Dictates if the "time with children" column is displayed by default, defaults to false.
    /// For a per-page override you can use .RenderIncludes(showTimeWithChildren: true/false)
    /// </summary>
    public bool PopupShowTimeWithChildren { get; set; } = false;

    /// <summary>
    /// Dictates if trivial timings are displayed by default, defaults to false.
    /// For a per-page override you can use .RenderIncludes(showTrivial: true/false)
    /// </summary>
    public bool PopupShowTrivial { get; set; } = false;

    public int PopupMaxTracesToShow { get; set; } = 15;

    public RenderPosition PopupRenderPosition { get; set; } = RenderPosition.Left;

    public string PopupToggleKeyboardShortcut { get; set; } = "Alt+P";

    public bool PopupStartHidden { get; set; } = false;

    public bool ShowControls { get; set; } = false;

    public HashSet<string> IgnoredDuplicateExecuteTypes { get; } = new()
    {
        "Open",
        "OpenAsync",
        "Close",
        "CloseAsync" // RelationalDiagnosticListener
    };

    public IAsyncStorage Storage { get; set; }

    public IAsyncProfilerProvider ProfilerProvider { get; set; } = new DefaultProfilerProvider();

    public Func<IStopwatch> StopwatchProvider { get; set; } = StopwatchWrapper.StartNew;


    public MiniProfiler StartProfiler(string profilerName = null)
    {
        return ProfilerProvider.Start(profilerName, this);
    }

    protected virtual void OnConfigure()
    {
    }

    internal void Configure()
    {
        OnConfigure();
    }
}