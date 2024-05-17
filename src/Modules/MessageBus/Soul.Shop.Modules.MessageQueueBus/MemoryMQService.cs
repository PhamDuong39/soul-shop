using MassTransit;
using Soul.Shop.Modules.MessageQueueBus.Abstractions;

namespace Soul.Shop.Modules.MessageQueueBus;

public class MemoryMQService(IBusControl busControl) : IMQService
{
    private readonly IBusControl _busControl = busControl;

    public async Task Send<T>(string queue, T message) where T : class
    {
        var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"loopback://localhost/{queue}"));
        await sendEndpoint.Send(message);
    }
}