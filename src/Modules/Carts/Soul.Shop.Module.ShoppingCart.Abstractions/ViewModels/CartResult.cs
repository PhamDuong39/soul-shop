namespace Soul.Shop.Module.ShoppingCart.Abstractions.ViewModels;

public class CartResult
{
    public int Id { get; set; }

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

    public decimal CheckedSubTotal
    {
        get { return Items.Where(x => x.IsChecked).Sum(x => x.Quantity * x.ProductPrice); }
    }

    public string CheckedSubTotalString => CheckedSubTotal.ToString("C");

    public decimal OrderTotal => SubTotal + (ShippingAmount ?? 0) - Discount;

    public string OrderTotalString => OrderTotal.ToString("C");

    public decimal CheckedOrderTotal => CheckedSubTotal + (ShippingAmount ?? 0) - Discount;

    public string CheckedOrderTotalString => CheckedOrderTotal.ToString("C");

    public int SubCount
    {
        get { return Items.Sum(x => x.Quantity); }
    }

    public int CheckedSubCount
    {
        get { return Items.Where(x => x.IsChecked).Sum(x => x.Quantity); }
    }

    public string OrderNote { get; set; }

    public IList<CartItemResult> Items { get; set; } = new List<CartItemResult>();
}