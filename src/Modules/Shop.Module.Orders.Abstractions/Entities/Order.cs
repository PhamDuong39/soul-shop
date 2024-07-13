using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Events;
using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.Entities;

public class Order : EntityBase
{
    public Order()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
        OrderStatus = OrderStatus.New;
        No = NoGen.Instance.GenOrderNo();
    }

    public long No { get; set; }

    public int CustomerId { get; set; }

    [JsonIgnore] // To simplify the json stored in order history
    public User Customer { get; set; }

    public int? ShippingAddressId { get; set; }

    public OrderAddress ShippingAddress { get; set; }

    public int? BillingAddressId { get; set; }

    public OrderAddress BillingAddress { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Transportation status
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

    /// <summary>
    /// Delivery time
    /// </summary>
    public DateTime? ShippedOn { get; set; }

    /// <summary>
    /// Delivery time
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    /// <summary>
    /// Buyer confirms the end time of receipt (T+7X is generated after the seller ships, and the buyer can extend the confirmation time of receipt, the maximum time after shipping + 60d)
    /// </summary>
    public DateTime? DeliveredEndOn { get; set; }

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
    /// Shipping method
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; }

    /// <summary>
    /// Shipping/shipping amount
    /// </summary>
    public decimal ShippingFeeAmount { get; set; }

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
    /// Buyer payment end time (buyer's remaining payment time T+120mX, generated when the buyer places an order)
    /// </summary>
    public DateTime? PaymentEndOn { get; set; }

    /// <summary>
    /// Coupon code, discount code
    /// </summary>
    public string CouponCode { get; set; }

    /// <summary>
    /// Coupon code, discount code rule name
    /// </summary>
    public string CouponRuleName { get; set; }

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

    /// <summary>
    /// Transaction closure/transaction cancellation reason
    /// The reasons you can choose are:
    /// 1. Failure to pay on time
    /// 2. Buyer does not want to buy
    /// 3. Buyer information is incorrect, re-bid
    /// 4. Malicious buyer/companion troublemaker
    /// 5. Out of stock
    /// 6. Buyer bid the wrong item
    /// 7. Meet-up transaction in the same city
    /// ...
    /// </summary>
    [StringLength(450)]
    public string CancelReason { get; set; }

    /// <summary>
    /// Suspended reason
    /// Frozen orders, orders in dispute, orders in refund
    /// </summary>
    [StringLength(450)]
    public string OnHoldReason { get; set; }

    /// <summary>
    /// Transaction closing/transaction cancellation time
    /// </summary>
    public DateTime? CancelOn { get; set; }

    public int CreatedById { get; set; }

    [JsonIgnore] public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    [JsonIgnore] public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public void AddOrderItem(OrderItem item)
    {
        item.Order = this;
        OrderItems.Add(item);
    }
}
