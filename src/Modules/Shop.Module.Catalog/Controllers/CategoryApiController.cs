using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Services;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Product classification API controller, responsible for product classification management operations, such as query, create, update and delete.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/categories")]
public class CategoryApiController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<ProductCategory> _productCategoryRepository;
    private readonly IRepository<Media> _mediaRepository;
    private readonly ICategoryService _categoryService;
    private readonly IMediaService _mediaService;

    public CategoryApiController(
        IRepository<Category> categoryRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IRepository<Media> mediaRepository,
        ICategoryService categoryService,
        IMediaService mediaService)
    {
        _categoryRepository = categoryRepository;
        _productCategoryRepository = productCategoryRepository;
        _mediaRepository = mediaRepository;
        _categoryService = categoryService;
        _mediaService = mediaService;
    }


    /// <summary>
    /// Get information about all product categories.
    /// </summary>
    /// <returns> Returns a list of information about all product categories. </returns>
    [HttpGet]
    public async Task<Result<IList<CategoryResult>>> Get()
    {
        var result = await _categoryService.GetAll();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get detailed information of the specified product category according to the product category ID.
    /// </summary>
    /// <param name="id">Product category ID. </param>
    /// <returns>Return detailed information of the specified product category. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var category = await _categoryRepository.Query().Include(x => x.Media).FirstOrDefaultAsync(c => c.Id == id);
        var model = new CategoryParam
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            MetaTitle = category.MetaTitle,
            MetaKeywords = category.MetaKeywords,
            MetaDescription = category.MetaDescription,
            DisplayOrder = category.DisplayOrder,
            Description = category.Description,
            ParentId = category.ParentId,
            IncludeInMenu = category.IncludeInMenu,
            IsPublished = category.IsPublished,
            MediaId = category.MediaId,
            ThumbnailImageUrl = await _mediaService.GetThumbnailUrl(category.Media)
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Get the product category list by page.
    /// </summary>
    /// <param name="param">An object containing paging and sorting parameters. </param>
    /// <returns>Return the paginated product category list. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<CategoryResult>>> List([FromBody] StandardTableParam param)
    {
        var result = await _categoryService.List(param);
        return result;
    }

    /// <summary>
    /// Clear the cache of product categories.
    /// </summary>
    /// <returns>Return the operation result. </returns>
    [HttpPost("clear-cache")]
    public async Task<Result> ClearCache()
    {
        await _categoryService.ClearCache();
        return Result.Ok();
    }

    /// <summary>
    /// Switch the display status of the specified product category in the menu.
    /// </summary>
    /// <param name="id">Product category ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPut("switch/{id:int:min(1)}")]
    public async Task<Result> SwitchInMenu(int id)
    {
        await _categoryService.SwitchInMenu(id);
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product category with the specified ID.
    /// </summary>
    /// <param name="id">Product category ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var category = await _categoryRepository.Query().Include(x => x.Children)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null)
            return Result.Fail("The document does not exist ");

        if (category.Children.Count > 0) return Result.Fail("Please make sure this category does not contain subcategories");

        await _categoryService.Delete(category);
        return Result.Ok();
    }

    /// <summary>
    /// Create a new product category.
    /// </summary>
    /// <param name="model">Parameter object containing product category information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPost]
    public async Task<Result> Create([FromBody] CategoryParam model)
    {
        var category = new Category
        {
            Name = model.Name,
            Slug = model.Slug,
            MetaTitle = model.MetaTitle,
            MetaKeywords = model.MetaKeywords,
            MetaDescription = model.MetaDescription,
            DisplayOrder = model.DisplayOrder,
            Description = model.Description,
            ParentId = model.ParentId,
            IncludeInMenu = model.IncludeInMenu,
            IsPublished = model.IsPublished,
            MediaId = model.MediaId
        };

        await _categoryService.Create(category);
        await _categoryService.ClearCache();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product category information of the specified ID.
    /// </summary>
    /// <param name="model">Parameter object containing the updated product category information. </param>
    /// <param name="id">Product category ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Update([FromBody] CategoryParam model, int id)
    {
        var category = await _categoryRepository.FirstOrDefaultAsync(id);
        if (category == null) return Result.Fail("The document does not exist");

        category.Name = model.Name;
        category.Slug = model.Slug;
        category.MetaTitle = model.MetaTitle;
        category.MetaKeywords = model.MetaKeywords;
        category.MetaDescription = model.MetaDescription;
        category.Description = model.Description;
        category.DisplayOrder = model.DisplayOrder;
        category.ParentId = model.ParentId;
        category.IncludeInMenu = model.IncludeInMenu;
        category.IsPublished = model.IsPublished;
        category.MediaId = model.MediaId;
        category.UpdatedOn = DateTime.Now;

        if (category.ParentId.HasValue && await HaveCircularNesting(category.Id, category.ParentId.Value))
            return Result.Fail("Parent category cannot be itself children");

        await _categoryService.Update(category);
        await _categoryService.ClearCache();
        return Result.Ok();
    }

    private async Task<bool> HaveCircularNesting(int childId, int parentId)
    {
        if (childId == parentId)
            return true;

        var categories = await _categoryService.GetAllByCache();
        var category = categories.FirstOrDefault(c => c.Id == parentId);
        var parentCategoryId = category?.ParentId;
        while (parentCategoryId.HasValue)
        {
            if (parentCategoryId.Value == childId) return true;
            var parentCategory = categories.FirstOrDefault(c => c.Id == parentCategoryId);
            parentCategoryId = parentCategory?.ParentId;
        }

        return false;
    }
}
