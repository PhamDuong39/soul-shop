using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

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

    public ShippingStatus? ShippingStatus { get; set; }

    public ShippingMethod ShippingMethod { get; set; }

    public decimal ShippingFeeAmount { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public decimal PaymentFeeAmount { get; set; }

    public DateTime? PaymentOn { get; set; }

    public decimal OrderTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public string OrderNote { get; set; }

    public string AdminNote { get; set; }

    public string CancelReason { get; set; }

    public DateTime? CancelOn { get; set; }

    public int CreatedById { get; set; }

    public int UpdatedById { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<OrderGetItemResult> Items { get; set; } = new List<OrderGetItemResult>();

    public OrderGetAddressResult BillingAddress { get; set; }

    public OrderGetAddressResult ShippingAddress { get; set; }

    public DateTime? ShippedOn { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public RefundStatus? RefundStatus { get; set; }

    public string RefundReason { get; set; }

    public DateTime? RefundOn { get; set; }

    public decimal RefundAmount { get; set; }
}