using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Reviews.Abstractions.Events;

namespace Soul.Shop.Modules.MessageQueueBus.Services
{
    public class ReviewAutoApprovedMQConsumer(
        ILogger<ReviewAutoApprovedMQConsumer> logger,
        IMediator mediator)
        : IConsumer<ReviewAutoApprovedEvent>
    {
        private readonly ILogger _logger = logger;

        public async Task Consume(ConsumeContext<ReviewAutoApprovedEvent> context)
        {
            try
            {
                if (context?.Message?.ReviewId > 0)
                {
                    await mediator.Publish(context.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Comments are automatically reviewed and processed, but processing fails", context?.Message);
            }
        }
    }
}

