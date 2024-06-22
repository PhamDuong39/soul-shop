// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Module.Core.Abstractions.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using Shop.Module.Schedule.Abstractions;

namespace Shop.Module.Core.Controllers;

/// <summary>
/// Mục đich dùng để xác thực tài khoản
/// </summary>
/// <param name="userManager"></param>
/// <param name="signInManager"></param>
/// <param name="smsSendRepository"></param>
/// <param name="emailSender"></param>
/// <param name="smsSender"></param>
/// <param name="loggerFactory"></param>
/// <param name="userRepository"></param>
/// <param name="tokenService"></param>
/// <param name="workContext"></param>
/// <param name="jobService"></param>
/// <param name="mediaRepository"></param>
/// <param name="accountService"></param>
/// <param name="config"></param>
[ApiController]
[Authorize]
[Route("api/account")]
public class AccountApiController(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IRepository<SmsSend> smsSendRepository,
    IEmailSender emailSender,
    ISmsSender smsSender,
    ILoggerFactory loggerFactory,
    IRepository<User> userRepository,
    ITokenService tokenService,
    IWorkContext workContext,
    IRedisService redisService,
    IJobService jobService,
    IRepository<Media> mediaRepository,
    IAccountService accountService,
    IOptionsMonitor<ShopOptions> config) : ControllerBase
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AccountApiController>();
    private readonly string _webHost = config.CurrentValue.WebHost;

    /// <summary>
    /// Get information of current user
    /// </summary>
    /// <returns></returns>
    [HttpGet()]
    public async Task<Result> CurrentUser()
    {
        var user = await workContext.GetCurrentUserAsync();
        if (user == null)
            return Result.Fail("Error when get user");
        var result = new AccountResult()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Culture = user.Culture,
            Email = StringHelper.EmailEncryption(user.Email),
            EmailConfirmed = user.EmailConfirmed,
            FullName = user.FullName,
            LastActivityOn = user.LastActivityOn,
            LastIpAddress = user.LastIpAddress,
            LastLoginOn = user.LastLoginOn,
            PhoneNumber = StringHelper.PhoneEncryption(user.PhoneNumber),
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            Avatar = user.AvatarUrl,
            NotifyCount = 20
        };
        return Result.Ok(result);
    }

    /// <summary>
    ///  Update current user
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPut()]
    public async Task<Result> PutCurrentUser(UserPutParam param)
    {
        var user = await workContext.GetCurrentUserAsync();
        if (user == null)
            return Result.Fail("Error when put user");

        if (param.MediaId.HasValue)
        {
            var media = await mediaRepository.FirstOrDefaultAsync(param.MediaId.Value);
            if (media != null)
            {
                user.AvatarId = media.Id;
                user.AvatarUrl = media.Url;
            }
        }

        user.FullName = param.FullName;
        await userManager.UpdateAsync(user);
        await signInManager.SignInAsync(user, false);
        return Result.Ok();
    }

    /// <summary>
    /// Create account by email password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register-by-email")]
    [AllowAnonymous]
    public async Task<Result> RegisterByPhone(RegisterByEmailParam model)
    {
        var any = userManager.Users.Any(c => c.Email == model.Email);
        if (any)
            return Result.Fail("this email has been registered");
        var user = new User
        {
            UserName = Guid.NewGuid().ToString("N"),
            FullName = model.Email,
            PhoneNumber = model.Phone,
            PhoneNumberConfirmed = true,
            IsActive = true,
            Culture = GlobalConfiguration.DefaultCulture
        };
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var verify = RegexHelper.VerifyEmail(model.Email);
            if (!verify.Succeeded)
                return Result.Fail(verify.Message);
            var anyEmail = userManager.Users.Any(c => c.Email == model.Email);
            if (anyEmail)
                return Result.Fail("This email is already in use");
            user.Email = model.Email;
            user.EmailConfirmed = false;
        }

        var transaction = userRepository.BeginTransaction();
        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return Result.Fail(result.Errors?.Select(c => c.Description).FirstOrDefault());
        }

        await userManager.AddToRoleAsync(user, RoleWithId.customer.ToString());
        await smsSendRepository.SaveChangesAsync();
        await transaction.CommitAsync();
        if (string.IsNullOrEmpty(user.Email))
        {
            return Result.Ok();
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendEmailConfirmation(user.Email, user.Id, code);

        return Result.Ok("Create account successfully");
    }

    /// <summary>
    /// Login by email with captcha
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("send-request-login-email-code")]
    [AllowAnonymous]
    public async Task<Result> LoginEmailGetCaptcha(LoginEmailGetCaptchaParam model)
    {
        if (!userManager.Users.Any(c => c.Email == model.Email))
            return Result.Fail("User does not exist");
        var code = CodeGen.GenRandomNumber();
        var result = await emailSender.SendEmailAsync(model.Email, "Captcha", code, false);
        //save code in redis and set expire time 15 p + email

        redisService.Set(model.Email, code, TimeSpan.FromMinutes(15));

        if (!result)
            return Result.Fail("Send email fail");
        return Result.Ok();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<Result> Login(LoginParam model, bool includeRefreshToken)
    {
        User user = null;
        if (RegexHelper.VerifyPhone(model.Name).Succeeded)
        {
            user = await userManager.Users.FirstOrDefaultAsync(c => c.PhoneNumber == model.Name);
            if (!user.PhoneNumberConfirmed)
                return Result.Fail("Phone number is not verified, not allowed to log in");
        }
        else if (RegexHelper.VerifyEmail(model.Name).Succeeded)
        {
            user = await userManager.FindByEmailAsync(model.Name);
            if (!user.EmailConfirmed)
                return Result.Fail("Email is not verified, not allowed to log in");
        }
        else
        {
            user = await userManager.FindByNameAsync(model.Name);
        }

        if (user == null)
            return Result.Fail("User does not exist");
        if (!user.IsActive)
            return Result.Fail("User is disabled");

        var signInResult = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
        if (signInResult.IsLockedOut)
        {
            return Result.Fail("User is locked, please try again later");
        }
        else if (signInResult.IsNotAllowed)
        {
            return Result.Fail("User email is not verified or phone is not verified, not allowed to log in");
        }
        else if (signInResult.RequiresTwoFactor)
        {
            var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);

            var ls = new List<(string, string)>();
            foreach (var item in userFactors)
                if (item == "Phone")
                    ls.Add((item, StringHelper.PhoneEncryption(user.PhoneNumber)));
                else if (item == "Email") ls.Add((item, StringHelper.EmailEncryption(user.Email)));
            return Result.Fail(
                new
                {
                    Providers = ls.Select(c => new { key = c.Item1, value = c.Item2 }),
                    signInResult.RequiresTwoFactor
                }, "Two-factor authentication is required");
        }
        else if (signInResult.Succeeded)
        {
            var token = await tokenService.GenerateAccessToken(user);
            var loginResult = new LoginResult()
            {
                Token = token,
                Avatar = user.AvatarUrl,
                Email = user.Email,
                Name = user.FullName,
                Phone = user.PhoneNumber
            };
            if (includeRefreshToken)
            {
                var refreshToken = tokenService.GenerateRefreshToken();
                user.RefreshTokenHash = userManager.PasswordHasher.HashPassword(user, refreshToken);
                await userManager.UpdateAsync(user);
                loginResult.RefreshToken = refreshToken;
            }

            return Result.Ok(loginResult);
        }

        return Result.Fail("Incorrect username or password");
    }
    /// <summary>
    /// Login by phone with captcha (not recommend)
    /// </summary>
    /// <param name="model"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpPost("login-verify-two-factor")]
    [AllowAnonymous]
    public async Task<Result> LoginVerify(LoginTwoFactorParam model)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return Result.Fail("Error");
        var userFactors = await userManager.GetValidTwoFactorProvidersAsync(user);
        if (userFactors.All(c => c != model.SelectedProvider)) return Result.Fail("Error");

        var code = await userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
        if (model.SelectedProvider == "Phone")
        {
            var phone = await userManager.GetPhoneNumberAsync(user);
            var send = await smsSender.SendCaptchaAsync(phone, code);
            if (!send.Success) return Result.Fail(send.Message);
        }
        else if (model.SelectedProvider == "Email")
        {
            var email = await userManager.GetEmailAsync(user);
            var message = "Your security code is: " + code;
            await emailSender.SendEmailAsync(email, "Security Code", message);
        }
        else
        {
            return Result.Fail("Error");
        }

        return Result.Ok();
    }

    /// <summary>
    /// Login two-factor authentication
    /// </summary>
    /// <param name="model"></param>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    [HttpPost("login-two-factor")]
    [AllowAnonymous]
    public async Task<Result> LoginTwoFactor(LoginTwoFactorParam model, string returnUrl = null)
    {
        var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return Result.Fail("Error");

        // The following code protects for brute force attacks against the two factor codes.
        // If a user enters incorrect codes for a specified amount of time then the user account
        // will be locked out for a specified amount of time.
        var result = await signInManager.TwoFactorSignInAsync(model.SelectedProvider, model.Code, model.RememberMe,
            model.RememberBrowser);
        if (result.IsLockedOut)
        {
            return Result.Fail("User is locked, please try again later");
        }
        else if (result.IsNotAllowed)
        {
            return Result.Fail("User email is not verified or phone is not verified, not allowed to log in");
        }
        else if (result.Succeeded)
        {

            await userManager.SetTwoFactorEnabledAsync(user, false);
            var token = await tokenService.GenerateAccessToken(user);
            return Result.Ok(new
            {
                token,
                name = user.FullName,
                phone = user.PhoneNumber,
                email = user.Email,
                returnUrl
            });
        }
        else
        {
            return Result.Fail("Incorrect code");
        }
    }

    /// <summary>
    /// Enable two-factor authentication
    /// </summary>
    /// <returns></returns>
    [HttpPost("enable-two-factor")]
    public async Task<Result> EnableTwoFactorAuthentication()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user != null)
        {
            await userManager.SetTwoFactorEnabledAsync(user, true);
            await signInManager.SignInAsync(user, false);
            _logger.LogInformation(1, "User enabled two-factor authentication.");
        }

        return Result.Ok();
    }

    /// <summary>
    /// disable two-factor authentication
    /// </summary>
    /// <returns></returns>
    [HttpPost("disable-two-factor")]
    public async Task<Result> DisableTwoFactorAuthentication()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user != null)
        {
            await userManager.SetTwoFactorEnabledAsync(user, false);
            await signInManager.SignInAsync(user, false);
            _logger.LogInformation(2, "User disabled two-factor authentication.");
        }

        return Result.Ok();
    }

    /// <summary>
    /// Send confirmation emai
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost("send-confirm-email")]
    public async Task<Result> SendConfirmEmail()
    {
        var currentUser = await workContext.GetCurrentUserAsync();
        if (currentUser == null)
            throw new Exception("User information is abnormal, please log in again");
        var user = await userManager.FindByIdAsync(currentUser.Id.ToString());
        if (user == null)
            throw new Exception("User information is abnormal, please log in again");

        if (string.IsNullOrWhiteSpace(user.Email))
            return Result.Fail("Email is not bound, please bind email first");
        if (user.EmailConfirmed)
            return Result.Fail("Email has been activated");
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await SendEmailConfirmation(user.Email, user.Id, code);
        return Result.Ok();
    }

    /// <summary>
    /// Confirmation email - activation email
    /// </summary>
    /// <param name="id"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPut("confirm-email")]
    [AllowAnonymous]
    public async Task<Result> ConfirmEmail([FromBody] ConfirmEmailParam param)
    {
        var user = await userManager.FindByIdAsync(param.UserId.ToString());
        if (user == null)
            return Result.Fail("The user does not exist or the link has expired");

        if (user.EmailConfirmed)
            return Result.Ok("Email is activate");

        var result = await userManager.ConfirmEmailAsync(user, param.Code);
        if (result.Succeeded)
            return Result.Ok();

        return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
    }

    /// <summary>
    /// change Password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("change-password")]
    public async Task<Result> ChangePassword(ChangePasswordParam model)
    {
        var currentUser = await workContext.GetCurrentUserAsync();
        if (currentUser == null)
            throw new Exception("User information is abnormal, please log in again");
        var user = await userManager.FindByIdAsync(currentUser.Id.ToString());
        if (user == null)
            throw new Exception("User information is abnormal, please log in again");
        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
            // await _signInManager.SignInAsync(user, isPersistent: false);
            await signInManager.SignOutAsync();
            tokenService.RemoveUserToken(user.Id);
            return Result.Ok();
        }

        return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
    }

    /// <summary>
    /// Forgot your password - How to retrieve your password (email or mobile phone)
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("forgot-password")]
    [AllowAnonymous]
    public async Task<Result<ForgotPasswordGetResult>> ForgotPassword(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Please enter your username, email, and mobile phone number");
        name = name.Trim();

        User user = null;
        if (RegexHelper.VerifyPhone(name).Succeeded)
            user = await userManager.Users.FirstOrDefaultAsync(c => c.PhoneNumber == name);
        else if (RegexHelper.VerifyEmail(name).Succeeded)
            user = await userManager.FindByEmailAsync(name);
        else
            user = await userManager.FindByNameAsync(name);
        if (user == null)
            throw new Exception("The user does not exist. Please confirm whether the user name, email address and mobile phone number are entered incorrectly.");
        if (!user.IsActive)
            throw new Exception("User disabled");
        if (string.IsNullOrWhiteSpace(user.PhoneNumber) && string.IsNullOrWhiteSpace(user.Email))
            throw new Exception("The user has not bound his email address and mobile phone, so he cannot retrieve his password. If you need help, please contact customer service.");

        var result = new ForgotPasswordGetResult()
        {
            UserName = user.UserName,
            Email = StringHelper.EmailEncryption(user.Email),
            Phone = StringHelper.PhoneEncryption(user.PhoneNumber)
        };
        return Result.Ok(result);
    }

    /// <summary>
    /// Email retrieval - send password reset email
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("forgot-password-email")]
    [AllowAnonymous]
    public async Task<Result> ForgotPasswordSendEmail([FromBody] ResetPasswordPostParam param)
    {
        var user = await userManager.FindByNameAsync(param.UserName);
        if (user == null)
            throw new Exception("user not found");
        if (!user.IsActive)
            throw new Exception("User disabled");
        if (string.IsNullOrWhiteSpace(user.Email))
            throw new Exception("The user is not bound to his email address and cannot retrieve his password through his email address.");

        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        // Send an email with this link
        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl =
            $"{_webHost.Trim('/')}/user/reset-password?userName={user.UserName}&email={StringHelper.EmailEncryption(user.Email)}&code={HttpUtility.UrlEncode(code)}";
        await jobService.Enqueue(() => emailSender.SendEmailAsync(user.Email, "Reset Password",
            $"Please reset your password by clicking here: <a href='{callbackUrl}'>REST PASSWORD</a>", true));
        return Result.Ok();
    }

    /// <summary>
    /// Email retrieval - reset password
    /// </summary>
    /// <param name="id"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPut("reset-password-email")]
    [AllowAnonymous]
    public async Task<Result> ResetPasswordByEmail([FromBody] ResetPasswordPutParam param)
    {
        var user = await userManager.FindByNameAsync(param.UserName);
        if (user == null)
            throw new Exception("User does not exist");
        if (!user.IsActive)
            throw new Exception("User disabled");

        var result = await userManager.ResetPasswordAsync(user, param.Code, param.Password);
        if (result.Succeeded)
        {
            if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;
                await userManager.UpdateAsync(user);
            }

            tokenService.RemoveUserToken(user.Id);
            return Result.Ok();
        }

        return Result.Fail(result.Errors?.FirstOrDefault()?.Description);
    }

    /// <summary>
    /// Phone retrieval - send verification code
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost("forgot-password-phone")]
    [AllowAnonymous]
    public async Task<Result> ForgotPasswordSendPhone([FromBody] ResetPasswordPostParam param)
    {
        var user = await userManager.FindByNameAsync(param.UserName);
        if (user == null)
            throw new Exception("User does not exist");
        if (!user.IsActive)
            throw new Exception("User disabled");
        if (string.IsNullOrWhiteSpace(user.PhoneNumber))
            throw new Exception("The user has not bound his mobile phone and cannot retrieve his password through his mobile phone.");

        var code = CodeGen.GenRandomNumber();
        var result = await smsSender.SendCaptchaAsync(user.PhoneNumber, code);
        if (!result.Success)
            return Result.Fail(result.Message);
        return Result.Ok();
    }


    /// <summary>
    /// Remove mobile phone binding
    /// </summary>
    /// <returns></returns>
    [HttpPost("remove-phone")]
    public async Task<Result> RemovePhoneNumber()
    {
        var user = await workContext.GetCurrentUserAsync();
        if (user != null)
        {
            var result = await userManager.SetPhoneNumberAsync(user, null);
            if (result.Succeeded)
                //await _signInManager.SignInAsync(user, isPersistent: false);
                return Result.Ok();
        }

        return Result.Fail("Unbinding failed");
    }

    /// <summary>
    /// Remove email binding
    /// </summary>
    /// <returns></returns>
    [HttpPost("remove-email")]
    public async Task<Result> RemoveEmail()
    {
        var user = await workContext.GetCurrentUserAsync();
        if (user != null)
        {
            var result = await userManager.SetEmailAsync(user, null);
            if (result.Succeeded)
                //await _signInManager.SignInAsync(user, isPersistent: false);
                return Result.Ok();
        }

        return Result.Fail("Unbinding failed");
    }


    /// <summary>
    /// add phone
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPut("add-phone")]
    public async Task<Result> AddPhone(AddPhoneParam param)
    {
        var currentUser = await workContext.GetCurrentUserAsync();
        var user = await userManager.FindByNameAsync(currentUser.UserName);
        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            return Result.Fail("The mobile phone has been bound and cannot be bound again");

        var any = userManager.Users.Any(c => c.PhoneNumber == param.Phone);
        if (any)
            return Result.Fail("This mobile number is already in use");

        var result = await userManager.ChangePhoneNumberAsync(user, param.Phone, param.Code);
        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, false);
            return Result.Ok();
        }

        return Result.Fail(result?.Errors?.FirstOrDefault()?.Description);
    }

    /// <summary>
    /// Add email binding - send binding link
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPost("add-email")]
    public async Task<Result> AddEmailSendToken(AddEmailPostParam param)
    {
        var currentUser = await workContext.GetCurrentUserAsync();
        var user = await userManager.FindByNameAsync(currentUser.UserName);
        if (!string.IsNullOrWhiteSpace(user.Email))
            return Result.Fail("Email has been bound and cannot be bound again");

        var verify = RegexHelper.VerifyEmail(param.Email);
        if (!verify.Succeeded)
            return Result.Fail(verify.Message);
        var anyEmail = userManager.Users.Any(c => c.Email == param.Email);
        if (anyEmail)
            return Result.Fail("This email is already in use");

        var code = await userManager.GenerateChangeEmailTokenAsync(user, param.Email);
        var webHost = _webHost.Trim('/');
        var callbackUrl =
            $"{webHost}/user/add-email?id={user.Id}&email={StringHelper.EmailEncryption(param.Email)}&code={HttpUtility.UrlEncode(code)}";
        await jobService.Enqueue(() => emailSender.SendEmailAsync(param.Email, "Confirm your account",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>VERIFY</a>", true));
        return Result.Ok();
    }

    /// <summary>
    /// Add email binding
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    [HttpPut("add-email")]
    [AllowAnonymous]
    public async Task<Result> AddEmail(AddEmailPutParam param)
    {
        //var currentUser = await _workContext.GetCurrentUser();
        var user = await userManager.FindByIdAsync(param.UserId.ToString());
        if (user == null)
            return Result.Fail("User does not exist");

        if (!string.IsNullOrWhiteSpace(user.Email))
            return Result.Fail("Email has been bound and cannot be bound again");

        var verify = RegexHelper.VerifyEmail(param.Email);
        if (!verify.Succeeded)
            return Result.Fail(verify.Message);
        var anyEmail = userManager.Users.Any(c => c.Email == param.Email);
        if (anyEmail)
            return Result.Fail("This email is already in use");

        var result = await userManager.ChangeEmailAsync(user, param.Email, param.Code);
        if (!result.Succeeded)
        {
            return Result.Fail(result?.Errors?.FirstOrDefault()?.Description);
        }

        await signInManager.SignInAsync(user, false);
        return Result.Ok();

    }

    /// <summary>
    /// sign out
    /// </summary>
    /// <returns></returns>
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<Result> LogOff()
    {
        var user = await workContext.GetCurrentUserAsync();
        await signInManager.SignOutAsync();
        if (user != null)
            tokenService.RemoveUserToken(user.Id);
        _logger.LogInformation(4, "User logged out.");
        return Result.Ok();
    }

    private async Task SendEmailConfirmation(string email, int userId, string code)
    {
        if (string.IsNullOrEmpty(email))
            return;
        var webHost = _webHost.Trim('/');
        var callbackUrl =
            $"{webHost}/user/confirm-email?id={userId}&email={StringHelper.EmailEncryption(email)}&code={HttpUtility.UrlEncode(code)}";
        await jobService.Enqueue(() => emailSender.SendEmailAsync(email, "Confirm your account",
            $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>VERIFY</a>", true));
    }
}
