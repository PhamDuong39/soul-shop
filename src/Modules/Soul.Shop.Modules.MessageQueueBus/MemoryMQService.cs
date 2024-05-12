using MassTransit;
using Soul.Shop.Modules.MessageQueueBus.Abstractions;

namespace Soul.Shop.Modules.MessageQueueBus
{
    public class MemoryMQService(IBusControl busControl) : IMQService
    {
        public async Task Send<T>(string queue, T message) where T : class
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri($"loopback://localhost/{queue}"));
            await sendEndpoint.Send(message);
        }
    }
}
