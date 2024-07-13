using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Models.Cache;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Extensions;

public class WorkContext : IWorkContext
{
    private const string UserGuidCookiesName = ShopKeys.UserGuidCookiesName;

    private User _currentUser;
    private readonly HttpContext _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<User> _userRepository;
    private readonly AuthenticationOptions _config;
    private readonly IStaticCacheManager _cacheManager;

    public WorkContext(
        IHttpContextAccessor contextAccessor,
        UserManager<User> userManager,
        IRepository<User> userRepository,
        IOptionsMonitor<AuthenticationOptions> config,
        IStaticCacheManager cacheManager)
    {
        _userManager = userManager;
        _httpContext = contextAccessor.HttpContext;
        _userRepository = userRepository;
        _config = config.CurrentValue;
        _cacheManager = cacheManager;
    }

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
        var abc = await _userManager.CreateAsync(_currentUser, ShopKeys.GuestDefaultPassword);
        await _userManager.AddToRoleAsync(_currentUser, RoleWithId.guest.ToString());
        SetUserGuidCookies(_currentUser.UserGuid);
        return _currentUser;
    }

    public async Task<User> GetCurrentOrThrowAsync()
    {
        var contextUser = await GetCurrentUserOrNullAsync();
        if (contextUser == null)
            throw new Exception("Please log in again");
        return _currentUser;
    }

    public async Task<User> GetCurrentUserOrNullAsync()
    {
        if (_currentUser != null) return _currentUser;

        var contextUser = _httpContext?.User;
        if (contextUser == null) return _currentUser;

        _currentUser = await _userManager.GetUserAsync(contextUser);

        if (_currentUser != null) return _currentUser;

        var userGuid = GetUserGuidFromCookies();
        if (userGuid.HasValue)
            _currentUser = _userRepository.Query().Include(x => x.Roles).FirstOrDefault(x => x.UserGuid == userGuid);

        if (_currentUser != null && _currentUser.Roles.Count == 1 &&
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

    /// <summary>
    /// Validate token and automatically renew
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <param name="statusCode"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool ValidateToken(int userId, string token, out int statusCode, string path = "")
    {
        statusCode = StatusCodes.Status200OK;

        if (userId <= 0 || string.IsNullOrWhiteSpace(token))
            return false;

        var _options = _config?.Jwt;
        if (_options == null)
            throw new ArgumentNullException(nameof(AuthenticationOptions));

        var key = ShopKeys.UserJwtTokenPrefix + userId;
        var currentUser = _cacheManager.Get<UserTokenCache>(key);
        if (currentUser != null && currentUser.Token.Equals(token, StringComparison.OrdinalIgnoreCase))
        {
            var utcNow = DateTime.UtcNow;
            var issuer = _options.Issuer;
            var jwtKey = _options.Key;
            var minutes = _options.AccessTokenDurationInMinutes;

            if (currentUser.TokenExpiresOnUtc != null && currentUser.TokenExpiresOnUtc < utcNow)
            {
                // expired
                _cacheManager.Remove(key);
                return false;
            }
            else if (minutes > 0 && currentUser.TokenExpiresOnUtc == null)
            {
                // When adjusting configurations, update configurations upon access (from no expiration time to having an expiration time)
                currentUser.TokenUpdatedOnUtc = utcNow;
                currentUser.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);

                _cacheManager.Set(key, currentUser, minutes);
            }
            else if (currentUser.TokenExpiresOnUtc != null &&
                     (utcNow - currentUser.TokenUpdatedOnUtc).TotalMinutes >= 1)
            {
                // Automatic renewal every minute.
                // By default, JWT tokens do not have an expiration policy enabled.
                currentUser.TokenUpdatedOnUtc = utcNow;
                currentUser.TokenExpiresOnUtc = utcNow.AddMinutes(minutes);

                _cacheManager.Set(key, currentUser, minutes);
            }

            return true;
        }
        else
        {
            // If the token does not exist or is inconsistent, return 401.
            statusCode = StatusCodes.Status401Unauthorized;
        }

        return false;
    }
}
