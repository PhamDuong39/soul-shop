using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateParam
{
    [Required] public int CustomerId { get; set; }

    public int? ShippingUserAddressId { get; set; }

    public int? BillingUserAddressId { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    public ShippingMethod ShippingMethod { get; set; }

    public decimal ShippingFeeAmount { get; set; }

    public decimal SubTotal { get; set; }

    public decimal SubTotalWithDiscount { get; set; }

    public decimal OrderTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    [StringLength(450)] public string OrderNote { get; set; }

    [StringLength(450)] public string AdminNote { get; set; }

    public IList<OrderCreateItemParam> Items { get; set; } = new List<OrderCreateItemParam>();

    public OrderCreateAddressParam BillingAddress { get; set; }

    public OrderCreateAddressParam ShippingAddress { get; set; }
}