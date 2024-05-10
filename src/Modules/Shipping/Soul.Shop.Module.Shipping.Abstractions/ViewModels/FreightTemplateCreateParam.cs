﻿using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Shipping.Abstractions.ViewModels;

public class FreightTemplateCreateParam
{
    [Required] [StringLength(450)] public string Name { get; set; }

    public string Note { get; set; }
}