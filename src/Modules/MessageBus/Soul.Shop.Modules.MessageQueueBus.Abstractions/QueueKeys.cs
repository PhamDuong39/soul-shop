using Soul.Shop.Module.Core.Abstractions.Data;

namespace Soul.Shop.Modules.MessageQueueBus.Abstractions;

public class QueueKeys : ShopKeys
{
    public static string ProductView = System + "_product_view";

    public static string ReviewAutoApproved = System + "_review_auto_approved";

    public static string ReplyAutoApproved = System + "_reply_auto_approved";

    public static string PaymentReceived = System + "_payment_received";
}