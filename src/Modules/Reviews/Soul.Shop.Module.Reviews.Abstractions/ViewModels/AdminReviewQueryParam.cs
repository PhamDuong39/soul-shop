using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class AdminReviewQueryParam
{
    public int? EntityId { get; set; }

    public EntityTypeWithId? EntityTypeId { get; set; }

    public ReviewStatus? Status { get; set; }

    public bool? IsMedia { get; set; }

    public int? Rating { get; set; }
}