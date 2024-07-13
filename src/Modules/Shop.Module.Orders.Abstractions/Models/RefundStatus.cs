namespace Shop.Module.Orders.Models;

/// <summary>
/// Refund status
/// </summary>
public enum RefundStatus
{
    /// <summary>
    /// Waiting for refund
    /// </summary>
    WaitRefund = 0,

    /// <summary>
    /// Refund successful
    /// </summary>
    RefundOk = 10,

    /// <summary>
    /// Refund canceled
    /// </summary>
    RefundCancel = 20,

    /// <summary>
    /// Close
    /// </summary>
    Close = 30,

    /// <summary>
    /// Refund frozen
    /// </summary>
    RefundFrozen = 40
}
