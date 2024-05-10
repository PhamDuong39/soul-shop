using Soul.Shop.Module.Core.Abstractions.ViewModels;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class CheckoutResult
{
    public string CouponCode { get; set; }

    public string CouponValidationErrorMessage { get; set; }

    public decimal Discount { get; set; }

    public string DiscountString => Discount.ToString("C");

    public decimal? ShippingAmount { get; set; }

    public string ShippingAmountString => ShippingAmount.HasValue ? ShippingAmount.Value.ToString("C") : "-";

    public decimal SubTotal
    {
        get { return Items.Sum(x => x.Quantity * x.ProductPrice); }
    }

    public string SubTotalString => SubTotal.ToString("C");

    public decimal OrderTotal => SubTotal + (ShippingAmount ?? 0) - Discount;

    public string OrderTotalString => OrderTotal.ToString("C");

    public int SubCount
    {
        get { return Items.Sum(x => x.Quantity); }
    }

    public string OrderNote { get; set; }

    public UserAddressShippingResult Address { get; set; }

    public IList<CheckoutItemResult> Items { get; set; } = new List<CheckoutItemResult>();
}