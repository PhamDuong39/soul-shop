using MediatR;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Core.Abstractions.Events;

public class EntityViewed : INotification
{
    public int EntityId { get; set; }

    public int UserId { get; set; }

    public EntityTypeWithId EntityTypeWithId { get; set; }
}