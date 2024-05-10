using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class EntityType : EntityBase
{
    [Required] [StringLength(450)] public string Name { get; set; }

    public bool IsMenuable { get; set; }

    [Required] [StringLength(450)] public string Module { get; set; }
}