﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class WidgetParamBase
{
    [Required] public string Name { get; set; }

    public int WidgetZoneId { get; set; }

    public DateTime? PublishStart { get; set; }

    public DateTime? PublishEnd { get; set; }

    public int DisplayOrder { get; set; }
}