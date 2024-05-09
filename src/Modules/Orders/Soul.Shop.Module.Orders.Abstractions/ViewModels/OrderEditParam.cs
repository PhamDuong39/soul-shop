using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderEditParam
{
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
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
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal PaymentFeeAmount { get; set; }
    public DateTime? PaymentOn { get; set; }
    public ShippingStatus? ShippingStatus { get; set; }
    public DateTime? ShippedOn { get; set; }
    public DateTime? DeliveredOn { get; set; }
    public RefundStatus? RefundStatus { get; set; }
    public string RefundReason { get; set; }
    public DateTime? RefundOn { get; set; }
    public decimal RefundAmount { get; set; }
    [StringLength(450)] public string CancelReason { get; set; }
    public DateTime? CancelOn { get; set; }
}