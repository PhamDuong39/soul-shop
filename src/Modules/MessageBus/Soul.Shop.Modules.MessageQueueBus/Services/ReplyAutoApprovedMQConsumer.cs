using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Reviews.Abstractions.Events;

namespace Soul.Shop.Modules.MessageQueueBus.Services;

public class ReplyAutoApprovedMQConsumer(
    ILogger<ReplyAutoApprovedMQConsumer> logger,
    IMediator mediator)
    : IConsumer<ReplyAutoApprovedEvent>
{
    private readonly ILogger _logger = logger;

    public async Task Consume(ConsumeContext<ReplyAutoApprovedEvent> context)
    {
        try
        {
            if (context?.Message?.ReplyId > 0) await mediator.Publish(context.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reply to automatic review message, processing failed", context?.Message);
        }
    }
}
