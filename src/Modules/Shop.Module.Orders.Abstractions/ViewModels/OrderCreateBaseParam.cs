using Shop.Module.Orders.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Orders.ViewModels;

public class OrderCreateBaseParam
{
    public int CustomerId { get; set; }

    public int ShippingUserAddressId { get; set; }

    public int? BillingUserAddressId { get; set; }

    public PaymentType PaymentType { get; set; } = PaymentType.OnlinePayment;

    /// <summary>
    /// Delivery method
    /// </summary>
    public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Free;

    /// <summary>
    /// Delivery/shipping fee amount
    /// </summary>
    public decimal ShippingFeeAmount { get; set; }

    /// <summary>
    /// Total order discount (shipping coupon, discount coupon, etc.)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Order notes
    /// </summary>
    [StringLength(450)]
    public string OrderNote { get; set; }

    public IList<OrderCreateBaseItemParam> Items { get; set; } = new List<OrderCreateBaseItemParam>();
}
