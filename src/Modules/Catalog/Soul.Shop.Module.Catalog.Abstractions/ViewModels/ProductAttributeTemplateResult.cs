﻿using System.Collections.Generic;
using System.Linq;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductAttributeTemplateResult
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<ProductAttributeResult> Attributes { get; set; } = new List<ProductAttributeResult>();

    public IList<int> AttributesIds => Attributes.Select(c => c.Id).ToArray();
}