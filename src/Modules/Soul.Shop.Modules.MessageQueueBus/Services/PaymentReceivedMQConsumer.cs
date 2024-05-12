using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Orders.Abstractions.Events;

namespace Soul.Shop.Modules.MessageQueueBus.Services
{
    public class PaymentReceivedMQConsumer(
        ILogger<PaymentReceivedMQConsumer> logger,
        IMediator mediator)
        : IConsumer<PaymentReceived>
    {
        private readonly ILogger _logger = logger;

        public async Task Consume(ConsumeContext<PaymentReceived> context)
        {
            try
            {
                if (context?.Message != null)
                {
                    await mediator.Publish(context.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Processing failed", context?.Message);
            }
        }
    }
}