using System.Reflection;
using Newtonsoft.Json;

namespace Soul.Shop.Infrastructure.Modules;

public class ModuleInfo
{
    [JsonProperty("id")] public string Id { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("version")] public Version Version { get; set; }

    public Assembly Assembly { get; set; }
}