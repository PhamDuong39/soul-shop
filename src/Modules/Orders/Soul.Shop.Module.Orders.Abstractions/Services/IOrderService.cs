using Soul.Shop.Module.Orders.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.ViewModels;

namespace Soul.Shop.Module.Orders.Abstractions.Services;

public interface IOrderService
{
    Task<Order> OrderCreate(int userId, OrderCreateBaseParam param, string adminNote = null);

    Task<OrderCreateResult> OrderCreateByCart(int cartId, OrderCreateBaseParam param, string adminNote = null);

    Task Cancel(int id, int userId, string reason);

    Task<OrderAddressResult> GetOrderAddress(int orderAddressId);

    Task<PaymentOrderBaseResponse> PayInfo(int orderId);

    Task PaymentReceived(PaymentReceivedParam param);

    Task<CheckoutResult> OrderCheckout(CheckoutParam param);
}