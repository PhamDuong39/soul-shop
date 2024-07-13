using Shop.Module.Core.Data;

namespace Shop.Module.MQ;

public class QueueKeys : ShopKeys
{
    /// <summary>
    /// Product browsing history message
    /// </summary>
    public static string ProductView = System + "_product_view";

    /// <summary>
    /// Automatic comment review message
    /// </summary>
    public static string ReviewAutoApproved = System + "_review_auto_approved";

    /// <summary>
    /// Automatic reply review message
    /// </summary>
    public static string ReplyAutoApproved = System + "_reply_auto_approved";

    /// <summary>
    /// Received payment message
    /// </summary>
    public static string PaymentReceived = System + "_payment_received";
}
