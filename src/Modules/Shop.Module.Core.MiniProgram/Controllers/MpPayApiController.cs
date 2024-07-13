using Essensoft.AspNetCore.Payment.WeChatPay;
using Essensoft.AspNetCore.Payment.WeChatPay.V2;
using Essensoft.AspNetCore.Payment.WeChatPay.V2.Notify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Module.Core.MiniProgram.Models;
using Shop.Module.MQ;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;

namespace Shop.Module.Core.MiniProgram.Controllers;

/// <summary>
/// Website payment API controller, used to handle notifications and requests related to website payments.
/// </summary>
[ApiController]
[Route("api/mp/pay")]
[Authorize()]
public class MpPayApiController : ControllerBase
{
    private readonly IWeChatPayNotifyClient _client;
    private readonly IMQService _mqService;
    private readonly ILogger _logger;
    private readonly MiniProgramOptions _options;

    public MpPayApiController(
        IWeChatPayNotifyClient client,
        IMQService mqService,
        ILogger<MpPayApiController> logger,
        IOptionsMonitor<MiniProgramOptions> options)
    {
        _client = client;
        _mqService = mqService;
        _logger = logger;
        _options = options.CurrentValue;
    }

    /// <summary>
    /// Receive and process asynchronous notifications after a successful website payment. Verify the authenticity of the notification and handle business logic such as updating order status, recording payment information, etc.
    /// </summary>
    /// <param name="no"> Order number, used to identify the order corresponding to the payment notification. </param>
    /// <returns> Return the processing result. If successful, return the expected success response to the WeChat server; otherwise, return an empty response. </returns>
    [AllowAnonymous]
    [HttpPost("notify/{no}")]
    public async Task<IActionResult> NotifyByOrderNo(string no)
    {
        try
        {
            var config = _options;
            var opt = new WeChatPayOptions()
            {
                AppId = config.AppId,
                MchId = config.MchId,
                AppSecret = config.AppSecret,
                Key = config.Key
            };

            var notify = await _client.ExecuteAsync<WeChatPayUnifiedOrderNotify>(Request, opt);
            if (notify.ReturnCode == "SUCCESS")
                if (notify.ResultCode == "SUCCESS")
                {
                    await _mqService.Send(QueueKeys.PaymentReceived, new PaymentReceived()
                    {
                        Note = "Website payment successful result notification",
                        OrderNo = no,
                        PaymentFeeAmount = notify.TotalFee / 100M,
                        PaymentMethod = PaymentMethod.WeChat,
                        PaymentOn = DateTime.ParseExact(notify.TimeEnd, "yyyyMMddHHmmss",
                            System.Globalization.CultureInfo.CurrentCulture)
                    });
                    return WeChatPayNotifyResult.Success;
                }

            return NoContent();
        }
        catch
        {
            return NoContent();
        }
        finally
        {
            _logger.LogInformation("Parameters：{@no}", no);
        }
    }
}
