﻿namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface IAppSettingService
{
    Task<string> Get(string key);

    Task<T> Get<T>();

    Task<T> Get<T>(string key);

    Task ClearCache(string key);
}