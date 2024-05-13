using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.Entities;

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

    [JsonIgnore]
    public User Customer { get; set; }

    public int? ShippingAddressId { get; set; }

    public OrderAddress ShippingAddress { get; set; }

    public int? BillingAddressId { get; set; }

    public OrderAddress BillingAddress { get; set; }
    public OrderStatus OrderStatus { get; set; }

    public PaymentType PaymentType { get; set; }

    public ShippingStatus? ShippingStatus { get; set; }

    public DateTime? ShippedOn { get; set; }

    public DateTime? DeliveredOn { get; set; }

    public DateTime? DeliveredEndOn { get; set; }

    public RefundStatus? RefundStatus { get; set; }

    public string RefundReason { get; set; }

    public DateTime? RefundOn { get; set; }

    public decimal RefundAmount { get; set; }

    public ShippingMethod ShippingMethod { get; set; }

    public decimal ShippingFeeAmount { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public decimal PaymentFeeAmount { get; set; }

    public DateTime? PaymentOn { get; set; }

    public DateTime? PaymentEndOn { get; set; }

    public string CouponCode { get; set; }

    public string CouponRuleName { get; set; }

    public decimal SubTotal { get; set; }

    public decimal SubTotalWithDiscount { get; set; }

    public decimal OrderTotal { get; set; }

    public decimal DiscountAmount { get; set; }

    [StringLength(450)] public string OrderNote { get; set; }

    [StringLength(450)] public string AdminNote { get; set; }

    [StringLength(450)] public string CancelReason { get; set; }

    [StringLength(450)] public string OnHoldReason { get; set; }

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