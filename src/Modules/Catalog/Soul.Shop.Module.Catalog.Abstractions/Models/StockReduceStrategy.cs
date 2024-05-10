namespace Soul.Shop.Module.Catalog.Abstractions.Models;

public enum StockReduceStrategy
{
    PlaceOrderWithhold = 0, // phần này là đặt hàng giữ hàng
    PaymentSuccessDeduct = 1 // phần này là thanh toán giảm hàng
}