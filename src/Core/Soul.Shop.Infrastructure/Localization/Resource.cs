﻿using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Infrastructure.Localization;

public class Resource : EntityBase
{
    [Required] [StringLength(450)] public string Key { get; set; }

    public string Value { get; set; }

    [Required] public string CultureId { get; set; }

    public Culture Culture { get; set; }
}