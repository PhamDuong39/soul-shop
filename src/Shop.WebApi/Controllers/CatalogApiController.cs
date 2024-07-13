using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Catalog.Services;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers;

/// <summary>
/// Bộ điều khiển API danh mục sản phẩm cung cấp các giao diện API liên quan đến danh mục sản phẩm.
/// </summary>
[ApiController]
[Route("api/catalogs")]
public class CatalogApiController
{
    private readonly ICategoryService _categoryService;

    /// <summary>
    /// Trình xây dựng, đưa vào giao diện dịch vụ danh mục sản phẩm.
    /// </summary>
    /// <param name="categoryService">Giao diện dịch vụ danh mục sản phẩm. </param>
    public CatalogApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Lấy thông tin về hai danh mục con.
    /// </summary>
    /// <returns>Kết quả phép toán chứa thông tin của hai danh mục con.</returns>
    [HttpGet()]
    public async Task<Result> GetTwoSubCategories()
    {
        var result = await _categoryService.GetTwoSubCategories();
        return Result.Ok(result);
    }

    /// <summary>
    /// Nhận thông tin về chỉ hai danh mục con trong ID danh mục chính.
    /// </summary>
    /// <param name="parentId">ID của danh mục chính, nếu trống sẽ là danh mục cao nhất.</param>
    /// <returns>Kết quả hoạt động, bao gồm thông tin danh mục con.</returns>
    [HttpGet("sub-categories")]
    public async Task<Result> GetTwoOnlyCategories(int? parentId = null)
    {
        var result = await _categoryService.GetTwoOnlyCategories(parentId);
        return Result.Ok(result);
    }
}
