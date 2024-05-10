namespace Soul.Shop.Infrastructure.Models;

public interface IEntityWithTypedId<out TKey>
{
    TKey Id { get; }
}