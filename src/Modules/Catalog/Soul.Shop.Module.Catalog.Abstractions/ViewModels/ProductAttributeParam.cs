﻿using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductAttributeParam
{
    public int Id { get; set; }

    [Required] public string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "The Attribute Group field is required.")]
    public int GroupId { get; set; }
}