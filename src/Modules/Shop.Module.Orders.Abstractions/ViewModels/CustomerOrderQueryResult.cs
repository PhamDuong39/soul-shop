using Shop.Infrastructure.Helpers;
using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Module.Orders.ViewModels;

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

    /// <summary>
    /// Shipping status
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

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
    /// Order total amount SubTotal + ShippingFeeAmount - SubTotalWithDiscount - DiscountAmount
    /// </summary>
    public decimal OrderTotal { get; set; }

    /// <summary>
    /// Order discount total (shipping coupon, discount coupon, etc.)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Order notes
    /// </summary>
    public string OrderNote { get; set; }

    /// <summary>
    /// Transaction closed/Transaction cancellation reason
    /// The reasons you can choose are:
    /// 1. Failure to pay on time
    /// 2. Buyer does not want to buy
    /// 3. Buyer’s information is incorrect, please bid again
    /// 4. Malicious buyer/companion disrupts
    /// 5. Out of stock
    /// 6. Buyer bid the wrong item
    /// 7. Meet-up transaction in the same city
    /// ...
    /// </summary>
    public string CancelReason { get; set; }

    /// <summary>
    /// Transaction closing/transaction cancellation time
    /// </summary>
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

    /// <summary>
    /// Total number of items
    /// </summary>
    public int ItemsTotal { get; set; }

    /// <summary>
    /// Quantity of commodity categories
    /// </summary>
    public int ItemsCount { get; set; }

    /// <summary>
    /// Buyer payment end time (buyer's remaining payment time T+120mX, generated when the buyer places an order)
    /// </summary>
    public DateTime? PaymentEndOn { get; set; }

    /// <summary>
    /// Buyer payment end time
    /// </summary>
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

    /// <summary>
    /// Buyer confirms the end time of receipt (generated after the seller ships T+7X)
    /// </summary>
    public DateTime? DeliveredEndOn { get; set; }

    /// <summary>
    /// Buyer confirms the end time of receipt
    /// </summary>
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
