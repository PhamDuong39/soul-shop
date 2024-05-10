using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Cache;

public class UserTokenCache
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public UserTokenType TokenType { get; set; } = UserTokenType.Default;
    public DateTime TokenCreatedOnUtc { get; set; }
    public DateTime TokenUpdatedOnUtc { get; set; }
    public DateTime? TokenExpiresOnUtc { get; set; }
}