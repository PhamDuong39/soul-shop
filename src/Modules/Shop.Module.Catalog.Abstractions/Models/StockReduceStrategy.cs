namespace Shop.Module.Catalog.Models;

/// <summary>
/// There are two types of inventory deduction strategies: place_order_withhold and payment_success_deduct.
/// </summary>
public enum StockReduceStrategy
{
    PlaceOrderWithhold = 0,
    PaymentSuccessDeduct = 1
}
