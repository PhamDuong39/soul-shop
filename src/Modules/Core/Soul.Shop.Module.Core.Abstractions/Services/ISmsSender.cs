using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Core.Abstractions.Services;

public interface ISmsSender
{
    Task<bool> SendSmsAsync(SmsSend model);
    Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha);
}