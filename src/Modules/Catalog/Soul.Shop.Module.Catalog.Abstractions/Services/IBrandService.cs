using Soul.Shop.Module.Catalog.Abstractions.ViewModels;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Web.StandardTable;
using Soul.Shop.Module.Catalog.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Services;

public interface IBrandService
{
    Task<IList<Brand>> GetAllByCache();

    Task Create(Brand brand);

    Task Update(Brand brand);

    Task<Result<StandardTableResult<BrandResult>>> List(StandardTableParam param);

    Task ClearCache();
}