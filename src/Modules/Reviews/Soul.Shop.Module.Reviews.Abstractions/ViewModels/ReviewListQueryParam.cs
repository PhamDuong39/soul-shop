using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Core.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReviewListQueryParam
{
    public int EntityId { get; set; }

    public EntityTypeWithId EntityTypeId { get; set; }

    [Range(1, 100)] public int Take { get; set; } = 1;
}