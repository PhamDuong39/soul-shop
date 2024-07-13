using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.SmsSenderAliyun.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Shop.Module.SmsSenderAliyun.Services;


/// <summary>
///
/// </summary>
public class AliyunSmsSenderService(
    ILoggerFactory loggerFactory,
    IOptionsMonitor<AliyunSmsOptions> options,
    IRepository<SmsSend> smsSendRepository,
    IStaticCacheManager cacheManager)
    : ISmsSender
{
    private const string SEPARATOR = "&";

    private readonly int timeoutInMilliSeconds = 100000;
    private readonly string version = "2017-05-25";
    private readonly string action = "SendSms";
    private readonly string format = "JSON";
    private readonly string domain = "dysmsapi.aliyuncs.com";
    private readonly string regionId = options.CurrentValue.RegionId;
    private readonly string accessKeyId = options.CurrentValue.AccessKeyId;
    private readonly string accessKeySecret = options.CurrentValue.AccessKeySecret;
    private readonly bool isTest = options.CurrentValue.IsTest;

    private readonly ILogger _logger = loggerFactory.CreateLogger<AliyunSmsSenderService>();

    public async Task<bool> SendSmsAsync(SmsSend model)
    {
        try
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (isTest || model.IsTest)
            {
                model.IsTest = true;
                model.IsSucceed = true;
                return true;
            }

            var paramers = new Dictionary<string, string>();
            paramers.Add("PhoneNumbers", model.PhoneNumber);
            paramers.Add("SignName", model.SignName);
            paramers.Add("TemplateCode", model.TemplateCode);
            paramers.Add("TemplateParam", model.TemplateParam);
            paramers.Add("AccessKeyId", accessKeyId);

            var url = GetSignUrl(paramers, accessKeySecret);
            var result = await HttpGetAsync(url);
            if (result.StatusCode == 200 && !string.IsNullOrEmpty(result.Response))
            {
                var message = JsonConvert.DeserializeObject<AliyunSendSmsResult>(result.Response);
                if (message?.Code == "OK")
                {
                    model.IsSucceed = true;
                    model.Message = message.Code;
                    model.ReceiptId = message.BizId;
                    return true;
                }
                else if (message != null)
                {
                    //smsRecord.
                    model.Message = message.Message;
                }
                else
                {
                    model.Message = result.Response;
                }
            }
            else
            {
                model.Message = "http status code: " + result.StatusCode + ", response: " + result.Response;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"send sms error: {ex.Message}", model);
            if (model != null) model.Message = ex.Message;
        }
        finally
        {
            if (model != null)
            {
                _logger.LogDebug($"sms: {JsonConvert.SerializeObject(model)}");
                smsSendRepository.Add(model);
                await smsSendRepository.SaveChangesAsync();
            }
        }

        return false;
    }

    public async Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentNullException(nameof(phone));
        if (string.IsNullOrWhiteSpace(captcha))
            throw new ArgumentNullException(nameof(captcha));
        phone = phone.Trim();
        captcha = captcha.Trim();
        var regex = new Regex(@"^\d{11}$");
        if (!regex.IsMatch(phone))
            return (false, "Invalid phone number");

        var cacheKey = ShopKeys.RegisterPhonePrefix + phone;
        if (cacheManager.IsSet(cacheKey)) return (false, "Verification code has been sent, please try again later.");

        var code = captcha;
        var success = await SendSmsAsync(new SmsSend()
        {
            PhoneNumber = phone,
            Value = code,
            TemplateType = SmsTemplateType.Captcha,
            TemplateCode = "SMS_70055704",
            SignName = "Shop",
            TemplateParam = JsonConvert.SerializeObject(new { code })
        });
        if (success)
        {
            cacheManager.Set(cacheKey, code, 1);
            return (true, "Submitted successfully");
        }

        return (false, "Send failed");
    }

    private static string SignString(string source, string accessSecret)
    {
        using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
        {
            return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
        }
    }

    private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
    {
        var imutableMap = new Dictionary<string, string>(parameters);
        imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
        imutableMap.Add("SignatureMethod", "HMAC-SHA1");
        imutableMap.Add("SignatureVersion", "1.0");
        imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
        imutableMap.Add("Action", action);
        imutableMap.Add("Version", version);
        imutableMap.Add("Format", format);
        imutableMap.Add("RegionId", regionId);

        IDictionary<string, string> sortedDictionary =
            new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
        var canonicalizedQueryString = new StringBuilder();
        foreach (var p in sortedDictionary)
            canonicalizedQueryString
                .Append("&")
                .Append(PercentEncode(p.Key)).Append("=")
                .Append(PercentEncode(p.Value));

        var stringToSign = new StringBuilder();
        stringToSign.Append("GET");
        stringToSign.Append(SEPARATOR);
        stringToSign.Append(PercentEncode("/"));
        stringToSign.Append(SEPARATOR);
        stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

        var signature = SignString(stringToSign.ToString(), accessSecret + "&");

        imutableMap.Add("Signature", signature);

        return ComposeUrl(domain, imutableMap);
    }

    private static string FormatIso8601Date(DateTime date)
    {
        return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
    }

    private static string ComposeUrl(string endpoint, Dictionary<string, string> parameters)
    {
        var urlBuilder = new StringBuilder("");
        urlBuilder.Append("http://").Append(endpoint);
        if (urlBuilder.ToString().IndexOf("?") == -1) urlBuilder.Append("/?");
        var query = ConcatQueryString(parameters);
        return urlBuilder.Append(query).ToString();
    }

    private static string ConcatQueryString(Dictionary<string, string> parameters)
    {
        if (null == parameters) return null;
        var sb = new StringBuilder();
        foreach (var entry in parameters)
        {
            var key = entry.Key;
            var val = entry.Value;
            sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
            if (val != null) sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
            sb.Append("&");
        }

        var strIndex = sb.Length;
        if (parameters.Count > 0)
            sb.Remove(strIndex - 1, 1);
        return sb.ToString();
    }

    private static string PercentEncode(string value)
    {
        var stringBuilder = new StringBuilder();
        var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        var bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
        foreach (char c in bytes)
            if (text.IndexOf(c) >= 0)
                stringBuilder.Append(c);
            else
                stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
        return stringBuilder.ToString();
    }

    private async Task<(int StatusCode, string Response)> HttpGetAsync(string url)
    {
        var handler = new HttpClientHandler();
        handler.Proxy = null;
        handler.AutomaticDecompression = DecompressionMethods.GZip;
        using (var http = new HttpClient(handler))
        {
            http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * timeoutInMilliSeconds);
            var response = await http.GetAsync(url);
            return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
    }
}
