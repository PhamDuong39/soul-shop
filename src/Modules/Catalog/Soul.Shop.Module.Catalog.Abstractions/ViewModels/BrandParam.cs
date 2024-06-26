﻿using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class BrandParam
{
    public BrandParam()
    {
        IsPublished = true;
    }

    public int Id { get; set; }

    [Required] public string Slug { get; set; }

    [Required] public string Name { get; set; }

    public string Description { get; set; }

    public bool IsPublished { get; set; }
}