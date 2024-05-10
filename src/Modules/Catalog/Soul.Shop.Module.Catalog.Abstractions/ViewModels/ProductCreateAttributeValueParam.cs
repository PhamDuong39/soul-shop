using System.Collections;
using System.Collections.Generic;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductCreateAttributeValueParam
{
    public int AttributeId { get; set; }

    public IList<string> Values { get; set; } = new List<string>();
}