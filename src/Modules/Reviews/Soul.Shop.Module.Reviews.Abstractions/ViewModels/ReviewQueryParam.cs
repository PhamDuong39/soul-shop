using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReviewQueryParam
{
    public int EntityId { get; set; }

    public EntityTypeWithId EntityTypeId { get; set; }

    public bool? IsMedia { get; set; }

    public RatingLevel? RatingLevel { get; set; }
}