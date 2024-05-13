using System.Diagnostics;
using Newtonsoft.Json;

namespace Soul.Shop.Module.ApiProfiler.Internal;

public class ResultRequest
{
    public Guid? Id { get; set; }

    public List<ClientTiming> Performance { get; set; }

    public List<ClientTiming> Probes { get; set; }

    public int? RedirectCount { get; set; }

    public int TimingCount => (Performance?.Count ?? 0) + (Probes?.Count ?? 0);

    private static readonly JsonSerializer _serializer = new();

    public static bool TryParse(Stream stream, out ResultRequest result)
    {
        try
        {
            using var sr = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(sr);
            var tmp = _serializer.Deserialize<ResultRequest>(jsonTextReader);
            if (tmp.Id.HasValue)
            {
                result = tmp;
                return true;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("Error parsing: " + e);
        }

        result = null;
        return false;
    }
}