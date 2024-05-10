﻿using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class ProvinceCreateParam
{
    public int? ParentId { get; set; }

    [StringLength(450)] public string Code { get; set; }

    [Required] [StringLength(450)] public string Name { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }
}