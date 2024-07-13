using Shop.Module.Orders.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels;

public class OrderCreateParam
{
    /// <summary>
    /// Customer
    /// </summary>
    [Required]
    public int CustomerId { get; set; }

    public int? ShippingUserAddressId { get; set; }

    public int? BillingUserAddressId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Shipping method
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; }

    /// <summary>
    /// Shipping/shipping amount
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
    /// Order discount total (shipping coupons, full discount coupons, etc.)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Order Notes
    /// </summary>
    [StringLength(450)]
    public string OrderNote { get; set; }

    /// <summary>
    /// Administrator Notes (internal use only)
    /// </summary>
    [StringLength(450)]
    public string AdminNote { get; set; }

    public IList<OrderCreateItemParam> Items { get; set; } = new List<OrderCreateItemParam>();

    public OrderCreateAddressParam BillingAddress { get; set; }

    public OrderCreateAddressParam ShippingAddress { get; set; }
}
