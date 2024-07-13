using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Brand management API controller, providing functions such as adding, deleting, modifying and checking brands.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/brands")]
public class BrandApiController : ControllerBase
{
    private readonly IRepository<Brand> _brandRepository;
    private readonly IBrandService _brandService;

    public BrandApiController(IRepository<Brand> brandRepository, IBrandService brandService)
    {
        _brandRepository = brandRepository;
        _brandService = brandService;
    }


    /// <summary>
    /// Get the brand list, support paging, sorting and other functions.
    /// </summary>
    /// <param name="param">Standard table parameters, including paging, sorting and other information. </param>
    /// <returns>Return the paginated brand list data. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<BrandResult>>> List([FromBody] StandardTableParam param)
    {
        var result = await _brandService.List(param);
        return result;
    }

    /// <summary>
    /// Get a brief information list of all brands.
    /// </summary>
    /// <returns> Return a brief information list of all brands. </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var brandList = await _brandService.GetAllByCache();
        return Result.Ok(brandList);
    }

    /// <summary>
    /// Get brand details based on brand ID.
    /// </summary>
    /// <param name="id">Brand ID. </param>
    /// <returns>Returns detailed information of the specified brand. If the brand does not exist, an error message is returned. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var brand = await _brandRepository.FirstOrDefaultAsync(id);
        if (brand == null) return Result.Fail("The document does not exist");
        var model = new BrandParam
        {
            Id = brand.Id,
            Name = brand.Name,
            Slug = brand.Slug,
            Description = brand.Description,
            IsPublished = brand.IsPublished
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Create a new brand.
    /// </summary>
    /// <param name="model">Parameter model containing brand information. </param>
    /// <returns>Return the operation result, indicating whether the brand is successfully created. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] BrandParam model)
    {
        var brand = new Brand
        {
            Name = model.Name,
            Slug = model.Slug,
            Description = model.Description,
            IsPublished = model.IsPublished
        };
        await _brandService.Create(brand);
        return Result.Ok();
    }

    /// <summary>
    /// Update the brand information of the specified ID.
    /// </summary>
    /// <param name="id">Brand ID. </param>
    /// <param name="model">Parameter model containing brand update information. </param>
    /// <returns>Return the operation result, indicating whether the brand information is successfully updated. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] BrandParam model)
    {
        var brand = await _brandRepository.FirstOrDefaultAsync(id);
        if (brand == null) return Result.Fail("The document does not exist");
        brand.Description = model.Description;
        brand.Name = model.Name;
        brand.Slug = model.Slug;
        brand.IsPublished = model.IsPublished;
        await _brandService.Update(brand);
        return Result.Ok();
    }

    /// <summary>
    /// Delete the brand with the specified ID.
    /// </summary>
    /// <param name="id">Brand ID. </param>
    /// <returns>Returns the operation result, indicating whether the brand is successfully deleted. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var brand = await _brandRepository.FirstOrDefaultAsync(id);
        if (brand == null) return Result.Fail("The document does not exist");
        brand.IsDeleted = true;
        brand.UpdatedOn = DateTime.Now;
        await _brandService.Update(brand);
        return Result.Ok();
    }

    /// <summary>
    /// Clear the brand cache.
    /// </summary>
    /// <returns>Returns the operation result, indicating whether the brand cache is successfully cleared. </returns>
    [HttpPost("clear-cache")]
    public async Task<Result> ClearCache()
    {
        await _brandService.ClearCache();
        return Result.Ok();
    }
}
