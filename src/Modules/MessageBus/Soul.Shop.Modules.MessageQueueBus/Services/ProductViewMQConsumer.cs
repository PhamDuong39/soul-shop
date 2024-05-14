using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Catalog.Abstractions.Events;
using Soul.Shop.Module.Core.Abstractions.Events;

namespace Soul.Shop.Modules.MessageQueueBus.Services;

public class ProductViewMQConsumer(
    ILogger<ProductViewMQConsumer> logger,
    IMediator mediator)
    : IConsumer<ProductViewed>
{
    private readonly ILogger _logger = logger;

    public async Task Consume(ConsumeContext<ProductViewed> context)
    {
        try
        {
            if (context?.Message != null)
                await mediator.Publish(new EntityViewed
                {
                    EntityId = context.Message.EntityId,
                    UserId = context.Message.UserId,
                    EntityTypeWithId = context.Message.EntityTypeWithId
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Product browsing record message, processing failed", context?.Message);
        }
    }
}