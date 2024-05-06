﻿using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface IMediaService
{
    Task<string> GetMediaUrl(Media media);

    Task<string> GetMediaUrl(string fileName);

    Task<string> GetThumbnailUrl(Media media);

    Task<Media> SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null, long fileSize = 0);

    Task DeleteMediaAsync(Media media);

    Task DeleteMediaAsync(string fileName);
}