﻿using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class WidgetZone : EntityBase
{
    public WidgetZone(int id)
    {
        Id = id;
        CreatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public string Description { get; set; }

    public DateTime CreatedOn { get; set; }
}