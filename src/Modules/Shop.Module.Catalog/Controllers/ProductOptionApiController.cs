using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Product options API controller, responsible for managing product options related operations.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/product-options")]
public class ProductOptionApiController : ControllerBase
{
    private readonly IRepository<ProductOption> _productOptionRepository;
    private readonly IRepository<ProductOptionData> _productOptionDataRepository;

    public ProductOptionApiController(
        IRepository<ProductOption> productOptionRepository,
        IRepository<ProductOptionData> productOptionDataRepository)
    {
        _productOptionRepository = productOptionRepository;
        _productOptionDataRepository = productOptionDataRepository;
    }

    /// <summary>
    /// Get all non-deleted product options.
    /// </summary>
    /// <returns>Product option list. </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var options = await _productOptionRepository.Query().Where(x => !x.IsDeleted).ToListAsync();
        return Result.Ok(options);
    }

    /// <summary>
    /// Get a paginated list of product options based on the pagination parameters.
    /// </summary>
    /// <param name="param">Pagination and filtering parameters. </param>
    /// <returns>The paginated list of product options. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<ProductOptionResult>>> DataList([FromBody] StandardTableParam param)
    {
        var query = _productOptionRepository.Query();
        var result = await query
            .ToStandardTableResult(param, x => new ProductOptionResult
            {
                Id = x.Id,
                Name = x.Name,
                DisplayType = x.DisplayType
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Get detailed information of a single product option based on the product option ID.
    /// </summary>
    /// <param name="id">Product option ID. </param>
    /// <returns>Details of the specified product option. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
        if (productOption == null)
            return Result.Fail("The document does not exist");
        var model = new ProductOptionResult
        {
            Id = productOption.Id,
            Name = productOption.Name,
            DisplayType = productOption.DisplayType
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Add a new product option.
    /// </summary>
    /// <param name="model">Product option data. </param>
    /// <returns>Operation results. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] ProductOptionParam model)
    {
        var productOption = new ProductOption
        {
            Name = model.Name,
            DisplayType = model.DisplayType
        };
        _productOptionRepository.Add(productOption);
        await _productOptionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product option with the specified ID.
    /// </summary>
    /// <param name="id">Product option ID. </param>
    /// <param name="model">Updated product option data. </param>
    /// <returns>Operation results. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] ProductOptionParam model)
    {
        var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
        if (productOption == null)
            return Result.Fail("The document does not exist");
        productOption.Name = model.Name;
        productOption.DisplayType = model.DisplayType;
        productOption.UpdatedOn = DateTime.Now;
        await _productOptionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product option with the specified ID.
    /// </summary>
    /// <param name="id">Product option ID. </param>
    /// <returns>Operation result. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var productOption = await _productOptionRepository.FirstOrDefaultAsync(id);
        if (productOption == null)
            return Result.Fail("The document does not exist");

        var any = _productOptionDataRepository.Query().Any(c => c.OptionId == id);
        if (any) return Result.Fail("Please make sure that the option is not referenced by value data");

        productOption.IsDeleted = true;
        productOption.UpdatedOn = DateTime.Now;
        await _productOptionRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Get all option data for the specified product option ID.
    /// </summary>
    /// <param name="optionId">Product option ID. </param>
    /// <returns>Product option data list. </returns>
    [HttpGet("data/{optionId:int:min(1)}")]
    public async Task<Result<List<ProductOptionDataListResult>>> DataList(int optionId)
    {
        var query = _productOptionDataRepository.Query(c => c.OptionId == optionId);
        var list = await query.Include(c => c.Option).ToListAsync();
        var result = list.Select(c => new ProductOptionDataListResult
        {
            Id = c.Id,
            Value = c.Value,
            Description = c.Description,
            OptionId = c.OptionId,
            OptionName = c.Option.Name,
            CreatedOn = c.CreatedOn,
            UpdatedOn = c.UpdatedOn,
            IsPublished = c.IsPublished,
            Display = c.Display,
            OptionDisplayType = c.Option.DisplayType
        }).ToList();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get a paginated list of product option data based on pagination parameters and product option ID.
    /// </summary>
    /// <param name="optionId">Product option ID. </param>
    /// <param name="param">Pagination and filtering parameters. </param>
    /// <returns>A paginated list of product option data. </returns>
    [HttpPost("data/{optionId:int:min(1)}/grid")]
    public async Task<Result<StandardTableResult<ProductOptionDataListResult>>> DataList(int optionId,
        [FromBody] StandardTableParam<ValueParam> param)
    {
        var query = _productOptionDataRepository.Query().Include(c => c.Option).Where(c => c.OptionId == optionId);
        if (param.Search != null)
        {
            var value = param.Search.Value;
            if (!string.IsNullOrWhiteSpace(value)) query = query.Where(x => x.Value.Contains(value.Trim()));
        }

        var result = await query.Include(c => c.Option)
            .ToStandardTableResult(param, c => new ProductOptionDataListResult
            {
                Id = c.Id,
                Value = c.Value,
                Description = c.Description,
                OptionId = c.OptionId,
                OptionName = c.Option.Name,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                IsPublished = c.IsPublished,
                Display = c.Display,
                OptionDisplayType = c.Option.DisplayType
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Add new option data to the specified product option.
    /// </summary>
    /// <param name="optionId">Product option ID. </param>
    /// <param name="model">Option data. </param>
    /// <returns>Operation results. </returns>
    [HttpPost("data/{optionId:int:min(1)}")]
    public async Task<Result> AddData(int optionId, [FromBody] ProductOptionDataParam model)
    {
        var data = new ProductOptionData
        {
            OptionId = optionId,
            IsPublished = model.IsPublished,
            Value = model.Value,
            Description = model.Description,
            Display = model.Display
        };
        _productOptionDataRepository.Add(data);
        await _productOptionDataRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product option data of the specified ID.
    /// </summary>
    /// <param name="id">Product option data ID. </param>
    /// <param name="model">Updated product option data. </param>
    /// <returns>Operation results. </returns>
    [HttpPut("data/{id:int:min(1)}")]
    public async Task<Result> EditData(int id, [FromBody] ProductOptionDataParam model)
    {
        var data = await _productOptionDataRepository.FirstOrDefaultAsync(id);
        if (data == null) return Result.Fail("The document does not exist");
        data.IsPublished = model.IsPublished;
        data.Value = model.Value;
        data.Description = model.Description;
        data.Display = model.Display;
        data.UpdatedOn = DateTime.Now;
        await _productOptionDataRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product option data of the specified ID.
    /// </summary>
    /// <param name="id">Product option data ID. </param>
    /// <returns>Operation results. </returns>
    [HttpDelete("data/{id:int:min(1)}")]
    public async Task<Result> DeleteData(int id)
    {
        var data = await _productOptionDataRepository.FirstOrDefaultAsync(id);
        if (data == null) return Result.Fail("The document does not exist");
        data.IsDeleted = true;
        data.UpdatedOn = DateTime.Now;
        await _productOptionDataRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
