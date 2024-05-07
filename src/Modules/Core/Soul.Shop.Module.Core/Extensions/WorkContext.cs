using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Module.Core.Cache;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Core.Abstractions.Data;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Extensions;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Core.Cache;

namespace Soul.Shop.Module.Core.Extensions;

public class WorkContext(
    IHttpContextAccessor contextAccessor,
    UserManager<User> userManager,
    IRepository<User> userRepository,
    IOptionsMonitor<AuthenticationOptions> config,
    IStaticCacheManager cacheManager)
    : IWorkContext
{
    private const string UserGuidCookiesName = ShopKeys.UserGuidCookiesName;

    private User _currentUser;
    private readonly HttpContext _httpContext = contextAccessor.HttpContext;
    private readonly AuthenticationOptions _config = config.CurrentValue;

    public async Task<User> GetCurrentUserAsync()
    {
        var contextUser = await GetCurrentUserOrNullAsync();
        if (contextUser != null)
            return contextUser;

        var userGuid = Guid.NewGuid();
        _currentUser = new User
        {
            FullName = RoleWithId.guest.ToString(),
            UserGuid = userGuid,
            UserName = userGuid.ToString("N"),
            Culture = GlobalConfiguration.DefaultCulture,
            IsActive = true
        };
        var abc = await userManager.CreateAsync(_currentUser, ShopKeys.GuestDefaultPassword);
        await userManager.AddToRoleAsync(_currentUser, RoleWithId.guest.ToString());
        SetUserGuidCookies(_currentUser.UserGuid);
        return _currentUser;
    }

    public async Task<User> GetCurrentOrThrowAsync()
    {
        var contextUser = await GetCurrentUserOrNullAsync();
        if (contextUser == null)
            throw new Exception("No current user found.");
        return _currentUser;
    }

    public async Task<User> GetCurrentUserOrNullAsync()
    {
        if (_currentUser != null) return _currentUser;

        var contextUser = _httpContext?.User;
        if (contextUser == null) return _currentUser;

        _currentUser = await userManager.GetUserAsync(contextUser);

        if (_currentUser != null) return _currentUser;

        var userGuid = GetUserGuidFromCookies();
        if (userGuid.HasValue)
            _currentUser = userRepository.Query().Include(x => x.Roles).FirstOrDefault(x => x.UserGuid == userGuid);

        if (_currentUser is { Roles.Count: 1 } &&
            _currentUser.Roles.First().RoleId == (int)RoleWithId.guest) return _currentUser;

        return _currentUser;
    }

    private Guid? GetUserGuidFromCookies()
    {
        if (_httpContext.Request.Cookies.ContainsKey(UserGuidCookiesName))
            return Guid.Parse(_httpContext.Request.Cookies[UserGuidCookiesName]);
        return null;
    }

    private void SetUserGuidCookies(Guid userGuid)
    {
        _httpContext.Response.Cookies.Append(UserGuidCookiesName, userGuid.ToString(), new CookieOptions
        {
            Expires = DateTime.UtcNow.AddYears(5),
            HttpOnly = true,
            IsEssential = true
        });
    }

    public bool ValidateToken(int userId, string token, out int statusCode, string path = "")
    {
        statusCode = StatusCodes.Status200OK;

        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
            return false;

        var _options = _config?.Jwt;
        if (_options == null)
            throw new ArgumentNullException(nameof(AuthenticationOptions));

        var key = ShopKeys.UserJwtTokenPrefix + userId;
        var currentUser = cacheManager.Get<UserTokenCache>(key);
        if (currentUser != null && currentUser.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
        {
            var utcNow = DateTime.UtcNow;
            var issuer = _options.Issuer;
            var jwtKey = _options.Key;
            var minutes = _options.AccessTokenDurationInMinutes;

            if (currentUser.TokenExpiresOnUtc != null && currentUser.TokenExpiresOnUtc < utcNow)
            {
                cacheManager.Remove(key);
                return false;
            }

            if (minutes > 0 && currentUser.TokenExpiresOnUtc == null)
            {
                currentUser.TokenUpdatedOnUtc = utcNow;
                currentUser.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);

                cacheManager.Set(key, currentUser, minutes);
            }
            else if (currentUser.TokenExpiresOnUtc != null &&
                     (utcNow - currentUser.TokenUpdatedOnUtc).TotalMinutes >= 1)
            {
                currentUser.TokenUpdatedOnUtc = utcNow;
                currentUser.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);

                cacheManager.Set(key, currentUser, minutes);
            }

            return true;
        }

        statusCode = StatusCodes.Status401Unauthorized;

        return false;
    }
}