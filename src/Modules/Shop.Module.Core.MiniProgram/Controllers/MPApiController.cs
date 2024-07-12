using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.MiniProgram.Data;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.Core.MiniProgram.ViewModels;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.MiniProgram.Controllers;

/// <summary>
/// Website API controller, used for handling requests related to websites, such as login
/// </summary>
[ApiController]
[Route("api/mp")]
public class MPApiController : ControllerBase
{
    private const string Code2SessionUrl = "https://api.weixin.qq.com/sns/jscode2session";
    private const string AccessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token";
    private const string WxaCodeUnlimited = "https://api.weixin.qq.com/wxa/getwxacodeunlimit";

    private readonly MiniProgramOptions _option;
    private readonly IRepository<UserLogin> _userLoginRepository;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;

    public MPApiController(
        IAppSettingService appSettingService,
        IAccountService accountService,
        IRepository<UserLogin> userLoginRepository,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ILoggerFactory loggerFactory,
        IConfiguration configuration,
        IRepository<User> userRepository,
        ITokenService tokenService,
        IOptionsMonitor<MiniProgramOptions> options)
    {
        _option = options.CurrentValue;
        _userLoginRepository = userLoginRepository;
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Handling login requests for websites. Communicating with the server using the code received from the website to obtain the user's unique identifier.
    /// If the user is logging in for the first time, a new user account will be created. Upon successful login, login information including the access token will be returned.
    /// </summary>
    /// <param name="param">The parameters containing the necessary information for website login, such as 'code'.</param>
    /// <returns>Return the operation result. If login is successful, it includes the user's login information such as access token and user details.</returns>
    [HttpPost("login")]
    public async Task<Result> Login([FromBody] LoginByMpParam param)
    {
        var url =
            $"{Code2SessionUrl}?appid={_option.AppId}&secret={_option.AppSecret}&js_code={param.Code}&grant_type=authorization_code";
        var httpClient = new HttpClient();
        var content = await httpClient.GetStringAsync(url);
        if (string.IsNullOrWhiteSpace(content)) return Result.Fail("Login failed");

        var result = JsonConvert.DeserializeObject<Code2SessionGetResult>(content);
        if (result.ErrCode != 0) return Result.Fail(result.ErrMessage);

        User user = null;
        var model = await _userLoginRepository.Query()
            .FirstOrDefaultAsync(c =>
                c.LoginProvider == MiniProgramDefaults.AuthenticationScheme && c.ProviderKey == result.OpenId);
        if (model == null)
        {
            // Create user
            var userName = Guid.NewGuid().ToString("N");
            user = new User
            {
                UserName = userName,
                FullName = param.NickName ?? userName,
                AvatarUrl = param.AvatarUrl,
                IsActive = true,
                Culture = GlobalConfiguration.DefaultCulture
            };
            var transaction = _userRepository.BeginTransaction();
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                transaction.Rollback();
                return Result.Fail("User creation failed");
            }

            await _userManager.AddToRoleAsync(user, RoleWithId.customer.ToString());
            _userLoginRepository.Add(new UserLogin()
            {
                LoginProvider = MiniProgramDefaults.AuthenticationScheme,
                ProviderDisplayName = MiniProgramDefaults.DisplayName,
                UserId = user.Id,
                ProviderKey = result.OpenId,
                UnionId = result.UnionId
            });
            await _userLoginRepository.SaveChangesAsync();
            transaction.Commit();
        }
        else
        {
            user = await _userManager.FindByIdAsync(model.UserId.ToString());
        }

        if (user == null) return Result.Fail("Login failed，Please try again later");

        // var providers = await _signInManager.GetExternalAuthenticationSchemesAsync();
        var signInResult =
            await _signInManager.ExternalLoginSignInAsync(MiniProgramDefaults.AuthenticationScheme, result.OpenId,
                false);

        if (signInResult.IsLockedOut)
        {
            return Result.Fail("The user is locked，Please try again later");
        }
        else if (signInResult.IsNotAllowed)
        {
            return Result.Fail("User email or phone number is not verified，Login is not permitted");
        }
        else if (signInResult.Succeeded)
        {
            var token = await _tokenService.GenerateAccessToken(user);
            var loginResult = new LoginResult()
            {
                Token = token,
                Avatar = user.AvatarUrl,
                Email = user.Email,
                Name = user.FullName,
                Phone = user.PhoneNumber
            };
            return Result.Ok(loginResult);
        }

        return Result.Fail("User login failed");
    }
}
