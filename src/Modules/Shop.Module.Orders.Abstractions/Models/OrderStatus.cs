using System.ComponentModel;

namespace Shop.Module.Orders.Models;

/// <summary>
/// Order status
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// New order
    /// </summary>
    [Description("New order")] New = 0,

    /// <summary>
    /// Suspended
    /// Frozen orders, orders in dispute, orders in refund
    /// </summary>
    [Description("Suspended")] OnHold = 10,

    /// <summary>
    /// Waiting for payment/pending payment
    /// </summary>
    [Description("Pending payment")] PendingPayment = 20,

    /// <summary>
    /// Payment failed
    /// Allow re-payment, equivalent to pending payment
    /// </summary>
    [Description("Payment failed")] PaymentFailed = 25,

    /// <summary>
    /// Payment received/paid
    /// </summary>
    [Description("paid")] PaymentReceived = 30,

    /// <summary>
    /// Shipping in progress
    /// Shipping status: pending, partially shipped
    /// </summary>
    [Description("Shipping")] Shipping = 40,

    /// <summary>
    /// Shipped
    /// Shipping status: no shipping, shipped
    /// </summary>
    [Description("Shipped")] Shipped = 50,

    /// <summary>
    /// Completed/transaction successful
    /// Shipping status: confirmed receipt
    /// </summary>
    [Description("transaction successful")] Complete = 60,

    /// <summary>
    /// Cancel/Close
    /// Buyers and sellers apply to cancel orders; transactions that are not paid on time are automatically canceled
    /// After payment, the user successfully refunds and the transaction is automatically closed/cancelled
    /// </summary>
    [Description("Transaction Cancelled")] Canceled = 70
}
