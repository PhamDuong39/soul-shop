using Soul.Shop.Module.Catalog.Abstractions.ViewModels;
using Soul.Shop.Module.Catalog.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Services;

public interface IProductPricingService
{
    CalculatedProductPrice CalculateProductPrice(GoodsListResult productThumbnail);

    CalculatedProductPrice CalculateProductPrice(Product product);

    CalculatedProductPrice CalculateProductPrice(decimal price, decimal? oldPrice, decimal? specialPrice,
        DateTime? specialPriceStart, DateTime? specialPriceEnd);
}