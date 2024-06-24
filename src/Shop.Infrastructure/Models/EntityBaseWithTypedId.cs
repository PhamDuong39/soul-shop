namespace Shop.Infrastructure.Models;

public abstract class EntityBaseWithTypedId<TId> : ValidatableObject, IEntityWithTypedId<TId>
{
    public virtual TId Id { get; set; } //protected set; redis Không thể đặt giá trị nếu được bảo vệ khi giải tuần tự hóa
}
