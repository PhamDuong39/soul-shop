using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Soul.Shop.Module.ApiProfiler;

internal class EmbeddedProvider(IOptions<MiniProfilerOptions> options, IHostingEnvironment env)
{
    /// <summary>
    /// Embedded resource contents keyed by filename.
    /// </summary>
    private ConcurrentDictionary<string, string> _resourceCache { get; } = new();

    private readonly IHostingEnvironment _env = env;
    private readonly IOptions<MiniProfilerOptions> _options = options;

    public string GetFile(HttpContext context, PathString file)
    {
        var response = context.Response;
        var path = file.Value;
        switch (Path.GetExtension(path))
        {
            case ".js":
                response.ContentType = "application/javascript";
                break;
            case ".css":
                response.ContentType = "text/css";
                break;
            default:
                return null;
        }

        if (TryGetResource(Path.GetFileName(path), out var resource)) return resource;

        return null;
    }

    public bool TryGetResource(string filename, out string resource)
    {
        filename = filename.ToLower();
        if (_resourceCache.TryGetValue(filename, out resource)) return true;

        // Fall back to embedded
        using (var stream = typeof(MiniProfiler).GetTypeInfo().Assembly
                   .GetManifestResourceStream("Shop.Module.ApiProfiler.ui." + filename))
        {
            if (stream == null) return false;
            using (var reader = new StreamReader(stream))
            {
                resource = reader.ReadToEnd();
            }
        }

        _resourceCache[filename] = resource;

        return true;
    }
}