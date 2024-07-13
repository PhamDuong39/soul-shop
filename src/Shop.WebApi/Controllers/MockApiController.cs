using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;

namespace Shop.WebApi.Controllers;

/// <summary>
/// Bộ điều khiển API mô phỏng chỉ được sử dụng trong môi trường phát triển và trang demo để mô phỏng quản trị viên cấp cao, người dùng người mua, đặt lại dữ liệu mẫu, đặt lại mật khẩu tài khoản thử nghiệm, v.v.
/// </summary>
[ApiController]
[Route("api/mock")]
public class MockApiController : ControllerBase
{
    private readonly IOptionsSnapshot<ShopOptions> _options;
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public MockApiController(IOptionsSnapshot<ShopOptions> options, IRepository<User> userRepository,
        ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _options = options;

        if (_options.Value.ShopEnv == ShopEnv.PRO) throw new Exception("Hoạt động này không được phép trong môi trường chính thức!");

        _userRepository = userRepository;
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Mô phỏng thông tin đăng nhập của người dùng siêu quản trị viên và tự động lưu mã thông báo vào cookie để vênh vang gọi trực tiếp giao diện mà không cần nhập.
    /// </summary>
    /// <returns></returns>
    [HttpGet("admin")]
    public async Task<Result> MockAdmin()
    {
        var user = _userRepository.Query(c => c.UserName == "admin").First();
        var token = await _tokenService.GenerateAccessToken(user);
        var loginResult = new LoginResult()
        {
            Token = token,
            Avatar = user.AvatarUrl,
            Email = user.Email,
            Name = user.FullName,
            Phone = user.PhoneNumber
        };

        // Làm mới quy trình Swagger để ghi nhớ trạng thái ủy quyền
        // Đặt Swagger để làm mới ủy quyền tự động

        Response.Cookies.Append("access-token", token,
            new CookieOptions() { SameSite = SameSiteMode.None, Secure = true });

        return Result.Ok(loginResult);
    }

    /// <summary>
    /// Đặt lại mật khẩu quản trị viên cấp cao thành 123456
    /// </summary>
    /// <returns></returns>
    [HttpGet("reset-admin-password")]
    public async Task<Result> MockResetAdminPassword()
    {
        var adminUser = _userRepository.Query(c => c.UserName == "admin").First();

        var user = await _userManager.FindByIdAsync(adminUser.Id.ToString());
        if (user == null)
            throw new Exception("Thông tin người dùng bất thường");

        var identityResult = await _userManager.RemovePasswordAsync(user);

        var result = await _userManager.AddPasswordAsync(user, "123456");
        if (result.Succeeded)
        {
            // await _signInManager.SignInAsync(user, isPersistent: false);

            await _signInManager.SignOutAsync();

            _tokenService.RemoveUserToken(user.Id);

            return Result.Ok();
        }

        return Result.Ok();
    }

    /// <summary>
    /// Mô phỏng thông tin đăng nhập thông thường người mua chỉ người dùng và tự động lưu mã thông báo vào cookie để vênh vang gọi trực tiếp
    /// giao diện mà không cần nhập (lưu ý: người dùng thông thường không thể gọi giao diện phụ trợ và sẽ trả về 403 Không có quyền)
    /// /// </summary>
    /// <returns></returns>
    [HttpGet("guest")]
    public async Task<Result> MockGuest()
    {
        var user = _userRepository.Query().Include(x => x.Roles)
            .FirstOrDefault(x => x.Roles.Any(x => x.RoleId == (int)RoleWithId.guest));

        var token = await _tokenService.GenerateAccessToken(user);
        var loginResult = new LoginResult()
        {
            Token = token,
            Avatar = user.AvatarUrl,
            Email = user.Email,
            Name = user.FullName,
            Phone = user.PhoneNumber
        };

        // Làm mới quy trình Swagger để ghi nhớ trạng thái ủy quyền
        // Đặt Swagger để làm mới ủy quyền tự động
        Response.Cookies.Append("access-token", token,
            new CookieOptions() { SameSite = SameSiteMode.None, Secure = true });

        return Result.Ok(loginResult);
    }
}
