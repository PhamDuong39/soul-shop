using System.Runtime.Serialization;
using Soul.Shop.Module.ApiProfiler.Internal;

namespace Soul.Shop.Module.ApiProfiler;

[DataContract]
public class ClientTimings
{
    [DataMember(Order = 2)] public List<ClientTiming> Timings { get; set; }

    [DataMember(Order = 1)] public int RedirectCount { get; set; }

    public static ClientTimings FromRequest(ResultRequest request)
    {
        var result = new ClientTimings()
        {
            RedirectCount = request.RedirectCount ?? 0,
            Timings = new List<ClientTiming>(request.Performance?.Count + request.Probes?.Count ?? 0)
        };
        if (request.Performance?.Count > 0)
            foreach (var t in request.Performance)
            {
                if (t.Name?.EndsWith("End") == true) continue;
                result.Timings.Add(t);
            }

        if (request.Probes?.Count > 0) result.Timings.AddRange(request.Probes);
        // Noise
        result.Timings.RemoveAll(t => t.Start < 0 || t.Duration < 0);
        // Sort for storage later
        result.Timings.Sort((a, b) => a.Start.CompareTo(b.Start));

        // TODO: Collapse client start/end timings? Probably...
        return result;
    }
}