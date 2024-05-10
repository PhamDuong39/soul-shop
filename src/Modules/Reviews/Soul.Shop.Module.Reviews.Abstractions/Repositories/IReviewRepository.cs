using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Reviews.Abstractions.Entities;
using Soul.Shop.Module.Reviews.Abstractions.ViewModels;

namespace Soul.Shop.Module.Reviews.Abstractions.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    IQueryable<ReviewListQueryDto> List();
}