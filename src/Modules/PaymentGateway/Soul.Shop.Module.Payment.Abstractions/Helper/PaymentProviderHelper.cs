using System.Security.Cryptography;

namespace Soul.Shop.Module.Payment.Abstractions.Helper;

public static class PaymentProviderHelper
{
    public static readonly string BraintreeProviderId = "Braintree";

    public static readonly string CashfreeProviderId = "Cashfree";

    public static readonly string CODProviderId = "CoD";

    public static readonly string MomoPaymentProviderId = "MomoPayment";

    public static readonly string NganLuongPaymentProviderId = "NganLuong";

    public static string GetToken(string message, string secretKey)
    {
        var encoding = new System.Text.ASCIIEncoding();
        var keyByte = encoding.GetBytes(secretKey);
        var messageBytes = encoding.GetBytes(message);
        using var hmacsha256 = new HMACSHA256(keyByte);
        var hashmessage = hmacsha256.ComputeHash(messageBytes);
        return Convert.ToBase64String(hashmessage);
    }
}
