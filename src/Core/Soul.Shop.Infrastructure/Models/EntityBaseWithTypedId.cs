namespace Soul.Shop.Infrastructure.Models;

public abstract class EntityBaseWithTypedId<TKey> : ValidatableObject, IEntityWithTypedId<TKey>
{
    public virtual TKey Id { get; set; }
}