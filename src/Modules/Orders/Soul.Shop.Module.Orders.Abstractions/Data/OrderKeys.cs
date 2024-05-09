using Soul.Shop.Module.Core.Abstractions.Data;

namespace Soul.Shop.Module.Orders.Abstractions.Data;

public class OrderKeys : ShopKeys
{
    public static string Module = System + ":order";

    public static string CustomerCreateOrderLock = Module + ":create:lock:";

    public const string OrderAutoCanceledTimeForMinute = "OrderAutoCanceledTimeForMinute";

    public const string OrderAutoCompleteTimeForMinute = "OrderAutoCompleteTimeForMinute";

    public const string OrderCompleteAutoReviewTimeForMinute = "OrderCompleteAutoReviewTimeForMinute";
}