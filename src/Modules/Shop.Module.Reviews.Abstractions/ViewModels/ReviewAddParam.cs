using Shop.Module.Core.Models;
using Shop.Module.Reviews.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Reviews.ViewModels;

public class ReviewAddParam
{
    [Range(1, 5)] public int Rating { get; set; }

    public string Title { get; set; }

    [Required(ErrorMessage = "Please enter a comment, length no more than 400")]
    [StringLength(400)]
    public string Comment { get; set; }

    public int EntityId { get; set; }

    public EntityTypeWithId EntityTypeId { get; set; } = EntityTypeWithId.Product;

    /// <summary>
    /// Comment source ID Example: Order ID
    /// </summary>
    public int? SourceId { get; set; set; }

    /// <summary>
    /// Comment source type Order = 0
    /// </summary>
    public ReviewSourceType? SourceType { get; set; }

    public bool IsAnonymous { get; set; }

    [MaxLength(9)] public IList<int> MediaIds { get; set; } = new List<int>();
}
