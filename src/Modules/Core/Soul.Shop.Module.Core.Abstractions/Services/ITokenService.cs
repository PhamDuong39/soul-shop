using System.Security.Claims;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface ITokenService
{
    Task<string> GenerateAccessToken(User user);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    bool ValidateToken(string identityId, string token);

    void RemoveUserToken(int userId);

    Task<IList<Claim>> BuildClaims(User user);
}