using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Reviews.Abstractions.Entities;

public class Support : EntityBase
{
    public Support()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int UserId { get; set; }

    public User User { get; set; }

    public int EntityTypeId { get; set; }

    public int EntityId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}