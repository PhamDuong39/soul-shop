using System.ComponentModel;

namespace Soul.Shop.Module.Orders.Abstractions.Models;

public enum OrderStatus
{
    [Description("New order")] New = 0,

    [Description("Hang")] OnHold = 10,

    [Description("Pending payment")] PendingPayment = 20,

    [Description("Payment Fail")] PaymentFailed = 25,

    [Description("Already paid")] PaymentReceived = 30,

    [Description("Shipping")] Shipping = 40,

    [Description("Shipped")] Shipped = 50,

    [Description("Transaction successful")] Complete = 60,

    [Description("Cancel transaction")] Canceled = 70
}