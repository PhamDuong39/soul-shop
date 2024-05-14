using Newtonsoft.Json;

namespace Soul.Shop.Module.StorageGitHub.Models;

public class GitHubDataResult
{
    [JsonProperty("content")] public GitHubDataContentResult Content { get; set; }
}
