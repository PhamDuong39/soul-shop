using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateBaseParam
{
    public int CustomerId { get; set; }

    public int ShippingUserAddressId { get; set; }

    public int? BillingUserAddressId { get; set; }

    public PaymentType PaymentType { get; set; } = PaymentType.OnlinePayment;

    public ShippingMethod ShippingMethod { get; set; } = ShippingMethod.Free;

    public decimal ShippingFeeAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    [StringLength(450)] public string OrderNote { get; set; }

    public IList<OrderCreateBaseItemParam> Items { get; set; } = new List<OrderCreateBaseItemParam>();
}