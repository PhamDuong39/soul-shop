using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Shipping.Abstractions.Entities;
using Shop.Module.Shipping.Entities;
using Shop.Module.Shipping.ViewModels;

namespace Shop.Module.Shipping.Controllers;

/// <summary>
/// Freight template API controller, responsible for managing operations related to the freight template.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/shippings/freight-templates")]
public class FreightTemplateApiController : ControllerBase
{
    private readonly IRepository<FreightTemplate> _freightTemplateRepository;
    private readonly IWorkContext _workContext;
    private readonly IRepository<StateOrProvince> _provinceRepository;
    private readonly IRepository<PriceAndDestination> _priceAndDestinationRepository;
    private readonly IRepository<Product> _productRepository;


    public FreightTemplateApiController(
        IRepository<FreightTemplate> freightTemplateRepository,
        IWorkContext workContext,
        IRepository<StateOrProvince> provinceRepository,
        IRepository<PriceAndDestination> priceAndDestinationRepository,
        IRepository<Product> productRepository)
    {
        _freightTemplateRepository = freightTemplateRepository;
        _workContext = workContext;
        _provinceRepository = provinceRepository;
        _priceAndDestinationRepository = priceAndDestinationRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get a list of all shipping patterns.
    /// </summary>
    /// <returns>List of shipping models.</returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var query = _freightTemplateRepository.Query();
        var result = await query.Select(x => new FreightTemplateQueryResult
        {
            Id = x.Id,
            Name = x.Name,
            Note = x.Note,
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn
        }).ToListAsync();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get a list of freight patterns in pages based on the given parameters.
    /// </summary>
    /// <param name="param">Paging and query parameters. </param>
    /// <returns>Paginated list of shipping templates.</returns>
    /// 
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<FreightTemplateQueryResult>>> DataList(
        [FromBody] StandardTableParam param)
    {
        var query = _freightTemplateRepository.Query();
        var result = await query
            .ToStandardTableResult(param, x => new FreightTemplateQueryResult
            {
                Id = x.Id,
                Name = x.Name,
                Note = x.Note,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Create a new shipping rate template.
    /// </summary>
    /// <param name="model">Creates the parameters of the freight model. </param>
    /// <returns>Create the results of the operation. </returns>
    /// 
    [HttpPost]
    public async Task<Result> Post([FromBody] FreightTemplateCreateParam model)
    {
        _freightTemplateRepository.Add(new FreightTemplate()
        {
            Note = model.Note,
            Name = model.Name
        });
        await _freightTemplateRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Updates the specified shipping template.
    /// </summary>
    /// <param name="id">ID of the shipping template to update.</param>
    /// <param name="model">Update parameters for freight model.</param>
    /// <returns>The result of the update operation. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] FreightTemplateCreateParam model)
    {
        var template = await _freightTemplateRepository.FirstOrDefaultAsync(id);
        if (template == null)
            return Result.Fail("Shipping template does not exist");
        template.Name = model.Name;
        template.Note = model.Note;
        template.UpdatedOn = DateTime.Now;
        await _freightTemplateRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the specified freight template.
    /// </summary>
    /// <param name="id">ID of the shipping sample to be deleted. </param>
    /// <returns> The result of the delete operation. </returns>
    /// 
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var template = await _freightTemplateRepository.FirstOrDefaultAsync(id);
        if (template == null)
            return Result.Fail("Mẫu vận chuyển hàng hóa không tồn tại");

        var any = _priceAndDestinationRepository.Query().Any(c => c.FreightTemplateId == id);
        if (any)
            return Result.Fail("Mẫu vận chuyển hàng hóa đã được tham chiếu và không được phép xóa.");

        var anyProduct = _productRepository.Query().Any(c => c.FreightTemplateId == id);
        if (anyProduct)
            return Result.Fail("Mẫu vận chuyển hàng hóa đã được sản phẩm tham chiếu và không được phép xóa.");

        template.IsDeleted = true;
        template.UpdatedOn = DateTime.Now;
        await _freightTemplateRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
