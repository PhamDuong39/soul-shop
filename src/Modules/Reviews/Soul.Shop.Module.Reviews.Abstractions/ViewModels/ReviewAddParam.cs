using System.ComponentModel.DataAnnotations;
using Soul.Shop.Module.Core.Abstractions.Models;
using Soul.Shop.Module.Reviews.Abstractions.Models;

namespace Soul.Shop.Module.Reviews.Abstractions.ViewModels;

public class ReviewAddParam
{
    [Range(1, 5)] public int Rating { get; set; }

    public string Title { get; set; }

    [Required(ErrorMessage = "Please enter a comment, the length cannot exceed 400")]
    [StringLength(400)]
    public string Comment { get; set; }

    public int EntityId { get; set; }

    public EntityTypeWithId EntityTypeId { get; set; } = EntityTypeWithId.Product;

    public int? SourceId { get; set; }

    public ReviewSourceType? SourceType { get; set; }

    public bool IsAnonymous { get; set; }

    [MaxLength(9)] public IList<int> MediaIds { get; set; } = new List<int>();
}