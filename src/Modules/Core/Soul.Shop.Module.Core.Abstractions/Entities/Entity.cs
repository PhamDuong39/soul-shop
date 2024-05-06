using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class Entity : EntityBase
{
    public Entity()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    [Required] [StringLength(450)] public string Slug { get; set; }

    public int EntityId { get; set; }

    public int EntityTypeId { get; set; }

    public EntityType EntityType { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}