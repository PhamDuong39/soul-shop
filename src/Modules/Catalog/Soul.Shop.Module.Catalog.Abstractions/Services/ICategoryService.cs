using Soul.Shop.Module.Catalog.Abstractions.ViewModels;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Web.StandardTable;
using Soul.Shop.Module.Catalog.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Services;

public interface ICategoryService
{

    Task<IList<CategoryResult>> GetAll();

    Task<IList<Category>> GetAllByCache();

    Task ClearCache();

    Task Create(Category category);

    Task Update(Category category);

    Task Delete(Category category);

    Task<Result<StandardTableResult<CategoryResult>>> List(StandardTableParam param);

    Task SwitchInMenu(int id);

    IList<CategoryResult> GetChildrens(int parentId, IList<CategoryResult> all);

    Task<IList<CategoryTwoSubResult>> GetTwoSubCategories(int? parentId = null, bool isPublished = true,
        bool includeInMenu = true);

    Task<IList<CategoryTwoSubResult>> GetTwoOnlyCategories(int? parentId = null, bool isPublished = true,
        bool includeInMenu = true);
}