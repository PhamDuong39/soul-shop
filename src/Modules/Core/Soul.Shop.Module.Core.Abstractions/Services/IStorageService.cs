using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface IStorageService
{
    Task<string> GetMediaUrl(string fileName);

    //Task<string> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null);

    Task DeleteMediaAsync(string fileName);

    Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null);
}