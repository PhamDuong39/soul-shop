using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Module.Core.Services;

public class AccountService : IAccountService
{
    private readonly IRepository<SmsSend> _smsSendRepository;

    public AccountService(IRepository<SmsSend> smsSendRepository)
    {
        _smsSendRepository = smsSendRepository;
    }

    /// <summary>
    /// Validate and retrieve the last verification code
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="captcha"></param>
    /// <returns></returns>
    public async Task<SmsSend> ValidateGetLastSms(string phone, string captcha)
    {
        // 5分钟内的验证码
        var startOn = DateTime.Now.AddMinutes(-5);
        var endOn = DateTime.Now;

        var sms = await _smsSendRepository.Query(c =>
                c.PhoneNumber == phone && c.IsSucceed && c.TemplateType == SmsTemplateType.Captcha &&
                c.CreatedOn <= endOn && c.CreatedOn >= startOn)
            .OrderByDescending(c => c.CreatedOn)
            .FirstOrDefaultAsync();

        if (sms == null || sms.IsUsed) throw new Exception("The verification code does not exist or has expired. Please obtain a new verification code");

        if (sms.Value != captcha) throw new Exception("The verification code is incorrect");

        return sms;
    }
}
