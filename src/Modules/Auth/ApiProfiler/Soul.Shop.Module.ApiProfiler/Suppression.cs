﻿using System.Runtime.Serialization;

namespace Soul.Shop.Module.ApiProfiler;

/// <summary>
/// An individual suppression block that deactivates profiling temporarily
/// </summary>
[DataContract]
public class Suppression : IDisposable
{
    private readonly bool _wasSuppressed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Suppression"/> class. 
    /// Obsolete - used for serialization.
    /// </summary>
    [Obsolete("Used for serialization")]
    public Suppression()
    {
        /* serialization only */
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Suppression"/> class. 
    /// Creates a new Suppression to deactivate profiling while alive
    /// </summary>
    /// <param name="profiler">The <see cref="Soul.Shop.Module.ApiProfiler.MiniProfiler"/> to suppress.</param>
    /// <exception cref="ArgumentNullException">Throws when the <paramref name="profiler"/> is <c>null</c>.</exception>
    public Suppression(MiniProfiler profiler)
    {
        Profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
        if (!Profiler.IsActive) return;

        Profiler.IsActive = false;
        _wasSuppressed = true;
    }

    /// <summary>
    /// Gets a reference to the containing profiler, allowing this Suppression to affect profiler activity.
    /// </summary>
    internal MiniProfiler Profiler { get; }

    void IDisposable.Dispose()
    {
        if (Profiler != null && _wasSuppressed) Profiler.IsActive = true;
    }
}