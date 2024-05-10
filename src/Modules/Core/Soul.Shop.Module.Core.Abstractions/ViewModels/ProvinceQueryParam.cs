﻿using Soul.Shop.Module.Core.Abstractions.Models;
using System.Collections.Generic;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class ProvinceQueryParam
{
    public int? ParentId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public IList<StateOrProvinceLevel> Level { get; set; } = new List<StateOrProvinceLevel>();
}