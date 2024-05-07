using System;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class GoodsListByRecentlyViewedResult : GoodsListResult
{
    public DateTime LatestViewedOn { get; set; }
}