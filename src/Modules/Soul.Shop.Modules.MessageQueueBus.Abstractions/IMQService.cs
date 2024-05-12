namespace Soul.Shop.Modules.MessageQueueBus.Abstractions;

public interface IMQService
{
    Task Send<T>(string queue, T message) where T : class;
}