namespace Soul.Shop.Module.StorageGitHub.Models;

public class StorageGitHubOptions
{
    public string Host { get; set; } = "https://api.github.com/";

    public string RepositoryName { get; set; }

    public string BranchName { get; set; }

    public string PersonalAccessToken { get; set; }

    public string SavePath { get; set; } = "/";
}
