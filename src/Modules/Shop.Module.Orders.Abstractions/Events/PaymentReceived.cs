using MediatR;
using Shop.Module.Orders.Models;
using System;

namespace Shop.Module.Orders.Events;

/// <summary>
/// Payment received/Payment successful
/// </summary>
public class PaymentReceived : INotification
{
    /// <summary>
    /// OrderId and OrderNo must be filled in one, if both are filled in, Id is used first
    /// </summary>
    public int? OrderId { get; set; }

    /// <summary>
    /// OrderId and OrderNo must be filled in one, if both are filled in, Id is used first
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// Payment method
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal? PaymentFeeAmount { get; set; }

    /// <summary>
    /// Payment time
    /// </summary>
    public DateTime? PaymentOn { get; set; }

    /// <summary>
    /// Note
    /// Mark payment
    /// WeChat applet payment callback notification
    /// </summary>
    public string Note { get; set; }
}
