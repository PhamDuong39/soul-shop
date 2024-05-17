using MediatR;
using Soul.Shop.Module.Orders.Abstractions.Events;
using Soul.Shop.Module.Orders.Abstractions.Services;
using Soul.Shop.Module.Orders.Abstractions.ViewModels;

namespace Soul.Shop.Module.Orders.Handlers;

public class PaymentReceivedHandler(IOrderService orderService) : INotificationHandler<PaymentReceived>
{
    public async Task Handle(PaymentReceived notification, CancellationToken cancellationToken)
    {
        if (notification == null)
            return;

        await orderService.PaymentReceived(new PaymentReceivedParam()
        {
            Note = notification.Note,
            OrderId = notification.OrderId,
            OrderNo = notification.OrderNo,
            PaymentFeeAmount = notification.PaymentFeeAmount,
            PaymentMethod = notification.PaymentMethod,
            PaymentOn = notification.PaymentOn
        });
    }
}