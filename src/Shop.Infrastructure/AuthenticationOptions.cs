namespace Shop.Infrastructure;

public class AuthenticationOptions
{
    public AuthenticationJwtConfig Jwt { get; set; } = new();
}

public class AuthenticationJwtConfig
{
    public string Key { get; set; }

    public string Issuer { get; set; }

    /// <summary>
    /// Mẹo: Chính sách hết hạn JWT chưa được bật. Thời gian này được sử dụng cho thời gian hết hạn tạo mã thông báo và thời gian hết hạn gia hạn mã thông báo.
    /// </summary>
    public int AccessTokenDurationInMinutes { get; set; }
}
