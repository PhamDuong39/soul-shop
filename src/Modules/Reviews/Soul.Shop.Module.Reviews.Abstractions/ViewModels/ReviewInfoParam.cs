using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReviewInfoParam
{
    public int EntityId { get; set; }

    public EntityTypeWithId EntityTypeId { get; set; }
}