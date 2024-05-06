namespace Soul.Shop.Infrastructure;

public class AuthenticationOptions
{
    public AuthenticationJwtConfig Jwt { get; set; } = new();
}

public class AuthenticationJwtConfig
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public int AccessTokenDurationInMinutes { get; set; }
}