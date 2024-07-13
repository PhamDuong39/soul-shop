using Shop.Infrastructure.Models;

namespace Shop.Infrastructure;

/// <summary>
/// Cấu hình cơ bản của trung tâm mua sắm
/// </summary>
public class ShopOptions
{
    /// <summary>
    /// Tên trung tâm thương mại
    /// </summary>
    public string ShopName { get; set; }

    /// <summary>
    /// Môi trường trung tâm thương mại DEV FAT UAT PRO
    /// </summary>
    public ShopEnv ShopEnv { get; set; } = ShopEnv.DEV;

    /// <summary>
    /// Bật điều chỉnh IP
    /// </summary>
    public bool IpRateLimitingEnabled { get; set; } = false;

    /// <summary>
    /// Bật điều tiết máy khách
    /// </summary>
    public bool ClientRateLimitingEnabled { get; set; } = false;

    /// <summary>
    /// Nhận hoặc đặt giá trị cho biết liệu chúng ta có nên sử dụng máy chủ Redis để lưu vào bộ nhớ đệm hay không (thay vì bộ nhớ đệm trong bộ nhớ mặc định)
    /// Redis hoặc Memeroy, hỗ trợ Redis (được sử dụng bởi các trang trại web, Azure, v.v.). Tìm thêm về nó tại https://azure.microsoft.com/en-us/documentation/articles/cache-dotnet-how-to-use-azure-redis-cache/
    /// </summary>
    public bool RedisCachingEnabled { get; set; } = false;

    /// <summary>
    /// Nhận hoặc đặt chuỗi kết nối Redis. Được sử dụng khi bộ nhớ đệm Redis được bật
    /// </summary>
    public string RedisCachingConnection { get; set; }

    /// <summary>
    /// Nhận thời gian bộ đệm mặc định tính bằng phút
    /// </summary>
    public int CacheTimeInMinutes { get; set; } = 60;

    public string ApiHost { get; set; }

    public string WebHost { get; set; }
}
