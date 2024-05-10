using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.Services;

public interface IReviewService
{
    Task ReviewAutoGood(int entityId, EntityTypeWithId entityTypeId, int? sourceId, ReviewSourceType? sourceType);
}