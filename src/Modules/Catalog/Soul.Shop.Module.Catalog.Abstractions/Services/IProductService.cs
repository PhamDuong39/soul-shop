using Soul.Shop.Module.Catalog.Abstractions.ViewModels;
using Soul.Shop.Infrastructure.Web.StandardTable;

namespace Soul.Shop.Module.Catalog.Abstractions.Services;

public interface IProductService
{
    Task<StandardTableResult<GoodsListResult>> HomeList(StandardTableParam<GoodsListQueryParam> param);

    Task<GoodsGetResult> GetGoodsByCache(int id);

    Task ClearGoodsCacheAndParent(int id);

    Task<IList<GoodsListResult>> RelatedList(int id);

    Task<IList<GoodsGetStockResult>> GetGoodsStocks(int id);
}