using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Helpers;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Core.Abstractions.Services;
using Soul.Shop.Module.StorageGitHub.Models;

namespace Soul.Shop.Module.StorageGitHub.Services;

public class GitHubStorageService(
    IOptionsMonitor<StorageGitHubOptions> options,
    IRepository<Media> mediaRepository)
    : IStorageService
{
    private const string ContentHost = "https://raw.githubusercontent.com";

    public async Task DeleteMediaAsync(string fileName)
    {
        await Task.CompletedTask;
    }

    public Task<string> GetMediaUrl(string fileName)
    {
        var optionsCurrentValue = options.CurrentValue;

        if (optionsCurrentValue == null || string.IsNullOrWhiteSpace(fileName))
            return Task.FromResult(string.Empty);
        var res = optionsCurrentValue.RepositoryName.Trim().Trim('/');
        var bra = optionsCurrentValue.BranchName.Trim().Trim('/');
        var path = optionsCurrentValue.SavePath.Trim().Trim('/');
        var pathInName = $"{fileName.Substring(0, 2)}/{fileName.Substring(2, 2)}/{fileName.Substring(4, 2)}";
        return Task.FromResult($"{ContentHost.TrimEnd('/')}/{res}/{bra}/{path}/{pathInName}/{fileName}");
    }

    public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
    {
        var bytes = new byte[mediaBinaryStream.Length];
        await using (mediaBinaryStream)
        {
            _ = await mediaBinaryStream.ReadAsync(bytes, 0, bytes.Length);
        }

        var hsMd5 = Md5Helper.Encrypt(bytes);
        var media = await mediaRepository.Query(c => c.Md5 == hsMd5).FirstOrDefaultAsync();
        if (media != null)
            return media;

        var result = await Task.Run(() => Upload(bytes, hsMd5, fileName));
        if (result?.Content == null ||
            string.IsNullOrWhiteSpace(result.Content.DownloadUrl)) return null;
        media = new Media()
        {
            MediaType = MediaType.File,
            FileName = result.Content.Name,
            FileSize = result.Content.Size,
            Hash = result.Content.Sha,
            Url = result.Content.DownloadUrl,
            Path = result.Content.Path?.Trim('/'),
            Host = await GetHostForUrlFrefix(),
            Md5 = hsMd5
        };
        if (!string.IsNullOrWhiteSpace(mimeType))
        {
            mimeType = mimeType.Trim().ToLower();
            if (mimeType.StartsWith("video"))
                media.MediaType = MediaType.Video;
            else if (mimeType.StartsWith("image"))
                media.MediaType = MediaType.Image;
            else
                media.MediaType = MediaType.File;
        }

        mediaRepository.Add(media);
        await mediaRepository.SaveChangesAsync();
        return media;
    }

    private async Task<GitHubDataResult> Upload(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentNullException(nameof(filePath));

        byte[] bytes;
        var uploadFileName = Path.GetFileName(filePath);
        await using (var fileStream = File.OpenRead(filePath))
        {
            bytes = new byte[fileStream.Length];
            _ = fileStream.Read(bytes, 0, bytes.Length);
        }

        var hsMd5 = Md5Helper.Encrypt(bytes);

        return await Upload(bytes, hsMd5, uploadFileName);
    }

    private async Task<string> GetHostForUrlFrefix()
    {
        var optionsCurrentValue = options.CurrentValue;
        if (optionsCurrentValue == null)
            return string.Empty;
        var res = optionsCurrentValue.RepositoryName.Trim().Trim('/');
        var bra = optionsCurrentValue.BranchName.Trim().Trim('/');
        return $"{ContentHost.TrimEnd('/')}/{res}/{bra}";
    }

    private async Task<GitHubDataResult> Upload(byte[] bytes, string hsMd5, string uploadFileName)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (string.IsNullOrWhiteSpace(hsMd5))
            throw new ArgumentNullException(nameof(hsMd5));
        if (uploadFileName == null)
            throw new ArgumentNullException(nameof(uploadFileName));

        var setting = options.CurrentValue;
        if (setting == null)
            throw new ArgumentNullException(nameof(setting));
        if (setting.Host == null)
            throw new ArgumentNullException(nameof(setting.Host));
        if (setting.RepositoryName == null)
            throw new ArgumentNullException(nameof(setting.RepositoryName));
        if (setting.BranchName == null)
            throw new ArgumentNullException(nameof(setting.BranchName));
        if (setting.PersonalAccessToken == null)
            throw new ArgumentNullException(nameof(setting.PersonalAccessToken));
        if (string.IsNullOrWhiteSpace(setting.SavePath))
            setting.SavePath = "/";

        var base64String = Convert.ToBase64String(bytes);

        var oriFileName = Path.GetFileName(uploadFileName);
        // .gif
        var extens = Path.GetExtension(oriFileName);
        var fileName = $"{hsMd5}{DateTime.Now.Ticks}{extens}";

        // images/4d/6d/5a
        var path =
            $"{setting.SavePath.TrimEnd('/')}/{fileName.Substring(0, 2)}/{fileName.Substring(2, 2)}/{fileName.Substring(4, 2)}"
                .TrimStart('/');

        var repo = setting.RepositoryName;
        var branch = setting.BranchName;
        var token = setting.PersonalAccessToken;
        var url = setting.Host + $"repos/{repo}/contents/{path}/{fileName}";
        var uri = new Uri(url);
        var body = new { message = "", branch = branch, content = base64String, path = $"{path}{fileName}" };

        var client = new RestClient(uri);
        var request = new RestRequest() { Method = Method.Put };
        request.AddHeader("Authorization", "token " + token);
        request.AddJsonBody(body);
        var response = client.Execute(request);

        if (response.StatusCode != HttpStatusCode.Created)
        {
            throw new Exception("Upload Error. " + response.Content);
        }

        var data = JsonConvert.DeserializeObject<GitHubDataResult>(response.Content);
        if (data is { Content: not null } && !string.IsNullOrWhiteSpace(data.Content.DownloadUrl))
            return data;
        throw new Exception("Upload Error. " + data?.Content);
    }
}
