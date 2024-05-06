using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Infrastructure;

public class ShopOptions
{
    public string ShopName { get; set; }

    public ShopEnv ShopEnv { get; set; } = ShopEnv.DEV;

    public bool IpRateLimitingEnabled { get; set; } = false;

    public bool ClientRateLimitingEnabled { get; set; } = false;

    public bool RedisCachingEnabled { get; set; } = false;

    public string RedisCachingConnection { get; set; }

    public int CacheTimeInMinutes { get; set; } = 60;

    public string ApiHost { get; set; }

    public string WebHost { get; set; }
}