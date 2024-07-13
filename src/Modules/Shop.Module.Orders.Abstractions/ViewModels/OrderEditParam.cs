using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels;

public class OrderEditParam
{
    public int? ShippingAddressId { get; set; }

    public int? BillingAddressId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Shipping method
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; }

    /// <summary>
    /// Shipping/shipping fee
    /// </summary>
    public decimal ShippingFeeAmount { get; set; }

    /// <summary>
    /// Order details Product total Sum(ProductPrice * Quantity)
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// Order details Discount total Sum(DiscountAmount)
    /// </summary>
    public decimal SubTotalWithDiscount { get; set; }

    /// <summary>
    /// Order total SubTotal + ShippingFeeAmount - SubTotalWithDiscount - DiscountAmount
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Order discount total (shipping coupons, discount coupons, etc.)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Order notes
    /// </summary>
    [StringLength(450)]
    public string OrderNote { get; set; }

    /// <summary>
    /// Admin Note (internal use only)
    /// </summary>
    [StringLength(450)]
    public string AdminNote { get; set; }

    public IList<OrderCreateItemParam> Items { get; set; } = new List<OrderCreateItemParam>();

    public OrderCreateAddressParam BillingAddress { get; set; }

    public OrderCreateAddressParam ShippingAddress { get; set; }

    /// <summary>
    /// Payment method
    /// </summary>
    public PaymentMethod? PaymentMethod { get; set; }

    /// <summary>
    /// Payment amount
    /// </summary>
    public decimal PaymentFeeAmount { get; set; }

    /// <summary>
    /// Payment time
    /// </summary>
    public DateTime? PaymentOn { get; set; }

    /// <summary>
    /// Shipping status
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

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

    /// <summary>
    /// Transaction closure/transaction cancellation reason
    /// The reasons you can choose are:
    /// 1. Failure to pay on time
    /// 2. Buyer does not want to buy
    /// 3. Buyer information is incorrect, re-bid
    /// 4. Malicious buyer/companion troublemaker
    /// 5. Out of stock
    /// 6. Buyer bid wrong
    /// 7. Meet-up transaction in the same city
    /// ...
    /// </summary>
    [StringLength(450)]
    public string CancelReason { get; set; }

    /// <summary>
    /// Transaction closure/transaction cancellation time
    /// </summary>
    public DateTime? CancelOn { get; set; }
}
