using Microsoft.OpenApi.Extensions;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class CustomerOrderQueryResult
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

    public string CancelReason { get; set; }

    public DateTime? CancelOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string OrderStatusString => OrderStatus.GetDisplayName();

    public decimal SubTotal
    {
        get { return Items.Sum(x => x.Quantity * x.ProductPrice); }
    }

    public string SubTotalString => SubTotal.ToString("C");

    public OrderAddressResult Address { get; set; }

    public IEnumerable<CustomerOrderItemQueryResult> Items { get; set; } = new List<CustomerOrderItemQueryResult>();

    public int ItemsTotal { get; set; }

    public int ItemsCount { get; set; }

    public DateTime? PaymentEndOn { get; set; }

    public int PaymentEndOnForSecond
    {
        get
        {
            if (PaymentEndOn.HasValue && PaymentEndOn > DateTime.Now &&
                (OrderStatus == OrderStatus.New || OrderStatus == OrderStatus.PendingPayment ||
                 OrderStatus == OrderStatus.PaymentFailed))
            {
                var totalSec = (PaymentEndOn - DateTime.Now).Value.TotalSeconds;
                if (totalSec > 0) return Convert.ToInt32(totalSec);
            }

            return 0;
        }
    }

    public DateTime? DeliveredEndOn { get; set; }

    public int DeliveredEndOnForSecond
    {
        get
        {
            if (DeliveredEndOn.HasValue && DeliveredEndOn > DateTime.Now &&
                (OrderStatus == OrderStatus.Shipping || OrderStatus == OrderStatus.Shipped) &&
                (ShippingStatus == Models.ShippingStatus.NoShipping ||
                 ShippingStatus == Models.ShippingStatus.PartiallyShipped ||
                 ShippingStatus == Models.ShippingStatus.Shipped))
            {
                var totalSec = (DeliveredEndOn - DateTime.Now).Value.TotalSeconds;
                if (totalSec > 0) return Convert.ToInt32(totalSec);
            }

            return 0;
        }
    }
}