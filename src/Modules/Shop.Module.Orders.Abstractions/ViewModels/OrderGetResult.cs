using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Orders.ViewModels;

public class OrderGetResult
{
    public int Id { get; set; }

    public string No { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; }

    public int? ShippingAddressId { get; set; }

    public int? BillingAddressId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Shipping status
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

    /// <summary>
    /// Delivery method
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; }

    /// <summary>
    /// Shipping/Freight Amount
    /// </summary>
    public decimal ShippingFeeAmount { get; set; }

    /// <summary>
    /// Payment Method
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Payment Amount
    /// </summary>
    public decimal PaymentFeeAmount { get; set; }

    /// <summary>
    /// Payment Time
    /// </summary>
    public DateTime? PaymentOn { get; set; }

    /// <summary>
    /// Total Order Amount SubTotal + ShippingFeeAmount - SubTotalWithDiscount - DiscountAmount
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Total order discount (shipping coupons, discount coupons, etc.)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Order notes
    /// </summary>
    public string OrderNote { get; set; }

    /// <summary>
    /// Administrator notes (internal use only)
    /// </summary>
    public string AdminNote { get; set; }

    /// <summary>
    /// Transaction closure/transaction cancellation reason
    /// The reasons you can choose are:
    /// 1. Failure to pay on time
    /// 2. The buyer does not want to buy
    /// 3. The buyer's information is incorrect, please bid again
    /// 4. Malicious buyers/companies make trouble
    /// 5. Out of stock
    /// 6. The buyer bid the wrong amount
    /// 7. Meet in person in the same city
    /// ...
    /// </summary>
    public string CancelReason { get; set; }

    /// <summary>
    /// Transaction closing/transaction cancellation time
    /// </summary>
    public DateTime? CancelOn { get; set; }

    public int CreatedById { get; set; }

    public int UpdatedById { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<OrderGetItemResult> Items { get; set; } = new List<OrderGetItemResult>();

    public OrderGetAddressResult BillingAddress { get; set; }

    public OrderGetAddressResult ShippingAddress { get; set; }

    /// <summary>
    /// Shipping time
    /// </summary>
    public DateTime? ShippedOn { get; set; }

    /// <summary>
    /// Receipt time
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    /// <summary>
    /// Refund status
    /// </summary>
    public RefundStatus? RefundStatus { get; set; }

    /// <summary>
    /// Refund reason
    /// </summary>
    public string RefundReason { get; set; }

    /// <summary>
    /// Refund time
    /// </summary>
    public DateTime? RefundOn { get; set; }

    /// <summary>
    /// Refund amount
    /// </summary>
    public decimal RefundAmount { get; set; }
}
