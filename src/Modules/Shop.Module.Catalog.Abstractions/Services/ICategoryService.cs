using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Module.Catalog.Services;

/// <summary>
/// Define the service interface for handling commodity classification related operations.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <returns>Category list. </returns>
    Task<IList<CategoryResult>> GetAll();

    /// <summary>
    /// Get all categories through cache.
    /// </summary>
    /// <returns>Category list. </returns>
    Task<IList<Category>> GetAllByCache();

    /// <summary>
    /// Clear the cache related to the category.
    /// </summary>
    Task ClearCache();

    /// <summary>
    /// Create a new category.
    /// </summary>
    /// <param name="category">Category entity to be created. </param>
    Task Create(Category category);

    /// <summary>
    /// Update an existing category.
    /// </summary>
    /// <param name="category">Category entity to be updated. </param>
    Task Update(Category category);

    /// <summary>
    /// Delete the specified category.
    /// </summary>
    /// <param name="category">Category entity to be deleted. </param>
    Task Delete(Category category);

    /// <summary>
    /// Get the category list, support paging and search.
    /// </summary>
    /// <param name="param">Paging and search parameters. </param>
    /// <returns>A list of categories that meet the conditions. </returns>
    Task<Result<StandardTableResult<CategoryResult>>> List(StandardTableParam param);

    /// <summary>
    /// Switch the display status of the category in the menu.
    /// </summary>
    /// <param name="id">The ID of the category. </param>
    Task SwitchInMenu(int id);

    /// <summary>
    /// Get all subcategories under the specified parent category.
    /// </summary>
    /// <param name="parentId">The ID of the parent category. </param>
    /// <param name="all">All available categories list. </param>
    /// <returns>Subcategories list. </returns>
    IList<CategoryResult> GetChildrens(int parentId, IList<CategoryResult> all);

    /// <summary>
    /// Get the first-level category and the corresponding second-level subcategories.
    /// </summary>
    /// <param name="parentId">Parent category ID. If null, get the top-level category and its subcategories. </param>
    /// <param name="isPublished">Whether to get only published categories. </param>
    /// <param name="includeInMenu">Whether to get only categories included in the menu. </param>
    /// <returns>Category list. </returns>
    Task<IList<CategoryTwoSubResult>> GetTwoSubCategories(int? parentId = null, bool isPublished = true,
    bool includeInMenu = true);

    /// <summary>
    /// Get only the second-level subcategories of the specified parent category.
    /// </summary>
    /// <param name="parentId">Parent category ID. If null, get the second-level subcategories of the top-level category. </param>
    /// <param name="isPublished">Whether to get only published categories. </param>
    /// <param name="includeInMenu">Whether to get only categories included in the menu. </param>
    /// <returns>Category list. </returns>
    Task<IList<CategoryTwoSubResult>> GetTwoOnlyCategories(int? parentId = null, bool isPublished = true,
    bool includeInMenu = true);
}
