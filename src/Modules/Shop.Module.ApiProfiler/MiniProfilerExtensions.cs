﻿using Shop.Module.ApiProfiler.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Shop.Module.ApiProfiler;

/// <summary>
/// Contains helper methods that ease working with null <see cref="MiniProfiler"/>s.
/// </summary>
public static class MiniProfilerExtensions
{
    /// <summary>
    /// Wraps <paramref name="selector"/> in a <see cref="Step(MiniProfiler, string)"/> call and executes it, returning its result.
    /// </summary>
    /// <typeparam name="T">the type of result.</typeparam>
    /// <param name="profiler">The current profiling session or null.</param>
    /// <param name="selector">Method to execute and profile.</param>
    /// <param name="name">The <see cref="Timing"/> step name used to label the profiler results.</param>
    /// <returns>the profiled result.</returns>
    /// <exception cref="ArgumentNullException">Throws when <paramref name="selector"/> is <c>null</c>.</exception>
    public static T Inline<T>(this MiniProfiler profiler, Func<T> selector, string name)
    {
        if (selector == null) throw new ArgumentNullException(nameof(selector));
        if (profiler == null) return selector();
        using (profiler.StepImpl(name))
        {
            return selector();
        }
    }

    /// <summary>
    /// Returns an <see cref="Timing"/> (<see cref="IDisposable"/>) that will time the code between its creation and disposal.
    /// </summary>
    /// <param name="profiler">The current profiling session or null.</param>
    /// <param name="name">A descriptive name for the code that is encapsulated by the resulting Timing's lifetime.</param>
    /// <returns>the profile step</returns>
    public static Timing Step(this MiniProfiler profiler, string name)
    {
        return profiler?.StepImpl(name);
    }

    /// <summary>
    /// Returns an <see cref="Timing"/> (<see cref="IDisposable"/>) that will time the code between its creation and disposal.
    /// Will only save the <see cref="Timing"/> if total time taken exceeds <paramref name="minSaveMs" />.
    /// </summary>
    /// <param name="profiler">The current profiling session or <c>null</c>.</param>
    /// <param name="name">A descriptive name for the code that is encapsulated by the resulting Timing's lifetime.</param>
    /// <param name="minSaveMs">The minimum amount of time that needs to elapse in order for this result to be recorded.</param>
    /// <param name="includeChildren">Should the amount of time spent in child timings be included when comparing total time
    /// profiled with <paramref name="minSaveMs"/>? If true, will include children. If false will ignore children.</param>
    /// <returns></returns>
    /// <remarks>If <paramref name="includeChildren"/> is set to true and a child is removed due to its use of StepIf, then the 
    /// time spent in that time will also not count for the current StepIf calculation.</remarks>
    public static Timing StepIf(this MiniProfiler profiler, string name, decimal minSaveMs,
        bool includeChildren = false)
    {
        return profiler?.StepImpl(name, minSaveMs, includeChildren);
    }

    /// <summary>
    /// Returns an <see cref="IDisposable"/> that will ignore profiling between its creation and disposal.
    /// </summary>
    /// <remarks>
    /// This is mainly useful in situations where you want to ignore database profiling for known hot spots,
    /// but it is safe to use in a nested step such that you can ignore sub-sections of a profiled step.
    /// </remarks>
    /// <param name="profiler">The current profiling session or null.</param>
    /// <returns>the profile step</returns>
    public static IDisposable Ignore(this MiniProfiler profiler)
    {
        return profiler != null ? new Suppression(profiler) : null;
    }

    /// <summary>
    /// Adds <paramref name="externalProfiler"/>'s <see cref="Timing"/> hierarchy to this profiler's current Timing step,
    /// allowing other threads, remote calls, etc. to be profiled and joined into this profiling session.
    /// </summary>
    /// <param name="profiler">The <see cref="MiniProfiler"/> to add to.</param>
    /// <param name="externalProfiler">The <see cref="MiniProfiler"/> to append to <paramref name="profiler"/>'s tree.</param>
    public static void AddProfilerResults(this MiniProfiler profiler, MiniProfiler externalProfiler)
    {
        if (profiler?.Head == null || externalProfiler == null) return;
        profiler.Head.AddChild(externalProfiler.Root);
    }

    /// <summary>
    /// Adds the <paramref name="text"/> and <paramref name="url"/> pair to <paramref name="profiler"/>'s 
    /// <see cref="MiniProfiler.CustomLinks"/> dictionary; will be displayed on the client in the bottom of the profiler popup.
    /// </summary>
    /// <param name="profiler">The <see cref="MiniProfiler"/> to add the link to.</param>
    /// <param name="text">The text label for the link.</param>
    /// <param name="url">The URL the link goes to.</param>
    public static void AddCustomLink(this MiniProfiler profiler, string text, string url)
    {
        if (profiler == null || !profiler.IsActive) return;

        lock (profiler)
        {
            profiler.CustomLinks = profiler.CustomLinks ?? new Dictionary<string, string>();
        }

        profiler.CustomLinks[text] = url;
    }

    /// <summary>
    /// Returns a plain-text representation of <paramref name="profiler"/>, suitable for viewing from 
    /// <see cref="Console"/>, log, or unit test output.
    /// </summary>
    /// <param name="profiler">A profiling session with child <see cref="Timing"/> instances.</param>
    /// <param name="htmlEncode">Whether to HTML encode the response, for use in a web page for example.</param>
    public static string RenderPlainText(this MiniProfiler profiler, bool htmlEncode = false)
    {
        if (profiler == null) return string.Empty;

        var text = StringBuilderCache.Get()
            .Append(htmlEncode ? WebUtility.HtmlEncode(Environment.MachineName) : Environment.MachineName)
            .Append(" at ")
            .Append(DateTime.UtcNow)
            .AppendLine();

        var timings = new Stack<Timing>();
        timings.Push(profiler.Root);

        while (timings.Count > 0)
        {
            var timing = timings.Pop();

            text.AppendFormat("{0} {1} = {2:###,##0.##}ms",
                new string('>', timing.Depth),
                htmlEncode ? WebUtility.HtmlEncode(timing.Name) : timing.Name,
                timing.DurationMilliseconds);

            if (timing.HasCustomTimings)
                foreach (var pair in timing.CustomTimings)
                {
                    var type = pair.Key;
                    var customTimings = pair.Value;

                    text.AppendFormat(" ({0} = {1:###,##0.##}ms in {2} cmd{3})",
                        type,
                        customTimings.Sum(ct => ct.DurationMilliseconds),
                        customTimings.Count,
                        customTimings.Count == 1 ? string.Empty : "s");
                }

            text.AppendLine();

            if (timing.HasChildren)
            {
                var children = timing.Children;
                for (var i = children.Count - 1; i >= 0; i--) timings.Push(children[i]);
            }
        }

        return text.ToStringRecycle();
    }
}