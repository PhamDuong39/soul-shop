using System.Collections.Generic;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductAttributeGroupArrayResult
{
    public int GroupId { get; set; }

    public string GroupName { get; set; }

    public IList<ProductAttributeResult> ProductAttributes { get; set; }
}