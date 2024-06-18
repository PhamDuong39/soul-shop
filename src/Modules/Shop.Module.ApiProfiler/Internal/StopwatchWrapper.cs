﻿using Shop.Module.ApiProfiler.Helpers;
using System.Diagnostics;

namespace Shop.Module.ApiProfiler.Internal;

/// <summary>
/// The stopwatch wrapper MiniProfile uses, for internal usage.
/// </summary>
public class StopwatchWrapper : IStopwatch
{
    private readonly Stopwatch _stopwatch;

    /// <summary>
    /// Initializes a new Stopwatch instance, sets the elapsed time property to zero, and starts measuring elapsed time.
    /// </summary>
    /// <returns>The <see cref="IStopwatch"/>.</returns>
    public static IStopwatch StartNew()
    {
        return new StopwatchWrapper();
    }

    /// <summary>
    /// Prevents a default instance of the <see cref="StopwatchWrapper"/> class from being created.
    /// </summary>
    private StopwatchWrapper()
    {
        _stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Gets the total elapsed time measured by the current instance, in timer ticks.
    /// </summary>
    public long ElapsedTicks => _stopwatch.ElapsedTicks;

    /// <summary>
    /// Gets the frequency of the timer as the number of ticks per second. This field is read-only.
    /// </summary>
    public long Frequency => Stopwatch.Frequency;

    /// <summary>
    /// Gets a value indicating whether the Stopwatch timer is running.
    /// </summary>
    public bool IsRunning => _stopwatch.IsRunning;

    /// <summary>
    /// Stops measuring elapsed time for an interval.
    /// </summary>
    public void Stop()
    {
        _stopwatch.Stop();
    }
}