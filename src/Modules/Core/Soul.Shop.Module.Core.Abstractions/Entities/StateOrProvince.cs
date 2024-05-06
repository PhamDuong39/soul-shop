﻿using Soul.Shop.Module.Core.Abstractions.Models;
using System;
using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class StateOrProvince : EntityBase
{
    public StateOrProvince()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int? ParentId { get; set; }

    public StateOrProvince Parent { get; set; }

    public int CountryId { get; set; }

    public Country Country { get; set; }

    [StringLength(450)] public string Code { get; set; }

    [Required] [StringLength(450)] public string Name { get; set; }

    public StateOrProvinceLevel Level { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}