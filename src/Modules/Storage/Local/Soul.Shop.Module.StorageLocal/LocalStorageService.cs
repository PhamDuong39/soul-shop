using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Helpers;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Core.Abstractions.Services;

namespace Soul.Shop.Module.StorageLocal;

public class LocalStorageService : IStorageService
{
    private const string MediaRootFolder = "user-content";

    private readonly string _host = "";
    private readonly IRepository<Media> _mediaRepository;

    public LocalStorageService(
        IRepository<Media> mediaRepository,
        IOptionsMonitor<ShopOptions> options)
    {
        _host = options.CurrentValue.ApiHost;
        _mediaRepository = mediaRepository;
    }

    public async Task DeleteMediaAsync(string fileName)
    {
        var filePath = Path.Combine(GlobalConfiguration.WebRootPath, MediaRootFolder, fileName);
        if (File.Exists(filePath)) await Task.Run(() => File.Delete(filePath));
    }

    public async Task<string> GetMediaUrl(string fileName)
    {
        if (fileName == GlobalConfiguration.NoImage) return await Task.FromResult($"{_host.Trim('/')}/{fileName}");

        return await Task.FromResult($"{_host.Trim('/')}/{MediaRootFolder}/{fileName}");
    }

    public async Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string? mimeType = null)
    {
        string hsMd5;
        var size = 0;
        Media? media = null;

        var filePath = Path.Combine(GlobalConfiguration.WebRootPath, MediaRootFolder, fileName);
        await using (var output = new FileStream(filePath, FileMode.Create))
        {
            //if (!File.Exists(filePath))

            await mediaBinaryStream.CopyToAsync(output);

            var bytes = new byte[mediaBinaryStream.Length];
            _ = await mediaBinaryStream.ReadAsync(bytes, 0, bytes.Length);
            hsMd5 = Md5Helper.Encrypt(bytes);
            size = bytes.Length;
        }

        if (!string.IsNullOrWhiteSpace(hsMd5))
            media = await _mediaRepository.Query(c => c.Md5 == hsMd5 || c.FileName == fileName).FirstOrDefaultAsync();

        if (media == null)
        {
            media = new Media()
            {
                MediaType = MediaType.File,
                FileName = fileName,
                FileSize = size,
                Hash = "",
                Url = $"{_host.Trim('/')}/{MediaRootFolder}/{fileName}",
                Path = $"/{MediaRootFolder}/{fileName}",
                Host = _host,
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

            _mediaRepository.Add(media);
        }
        else
        {
            media.Url = $"{_host.Trim('/')}/{MediaRootFolder}/{fileName}";
            media.Path = $"/{MediaRootFolder}/{fileName}";
            media.Host = _host;
        }

        await _mediaRepository.SaveChangesAsync();
        return media;
    }
}
