using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Reviews.Abstractions.Entities;

public class ReviewMedia : EntityBase
{
    public ReviewMedia()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int ReviewId { get; set; }

    public Review Review { get; set; }

    public int MediaId { get; set; }

    public Media Media { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}