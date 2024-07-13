using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Core.Models;
using Shop.Module.Shipping.Abstractions.Entities;
using Shop.Module.Shipping.Entities;
using Shop.Module.Shipping.ViewModels;

namespace Shop.Module.Shipping.Controllers;

/// <summary>
/// The rate and destination API controller is responsible for managing the specific freight rules in the freight template.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/shippings/price-destinations")]
public class PriceAndDestinationApiController : ControllerBase
{
    private readonly IRepository<FreightTemplate> _freightTemplateRepository;
    private readonly IWorkContext _workContext;
    private readonly IRepository<StateOrProvince> _provinceRepository;
    private readonly IRepository<PriceAndDestination> _priceAndDestinationRepository;

    public PriceAndDestinationApiController(
        IRepository<FreightTemplate> freightTemplateRepository,
        IWorkContext workContext,
        IRepository<StateOrProvince> provinceRepository,
        IRepository<PriceAndDestination> priceAndDestinationRepository)
    {
        _freightTemplateRepository = freightTemplateRepository;
        _workContext = workContext;
        _provinceRepository = provinceRepository;
        _priceAndDestinationRepository = priceAndDestinationRepository;
    }

    /// <summary>
    /// Get a list of freight strategies in pages based on the freight template ID.
    /// </summary>
    /// <param name="freightTemplateId">Freight template ID. </param>
    /// <param name="param">Paging parameters. </param>
    /// <returns> Paged list of shipping strategies. </return>

    [HttpPost("grid/{freightTemplateId:int:min(1)}")]
    public async Task<Result<StandardTableResult<PriceAndDestinationQueryResult>>> DataList(int freightTemplateId,
        [FromBody] StandardTableParam param)
    {
        var query = _priceAndDestinationRepository
            .Query(c => c.FreightTemplateId == freightTemplateId);
        StateOrProvinceLevel? level = null;
        var result = await query
            .Include(c => c.Country)
            .Include(c => c.StateOrProvince)
            .ToStandardTableResult(param, x => new PriceAndDestinationQueryResult
            {
                Id = x.Id,
                CountryId = x.CountryId,
                FreightTemplateId = x.FreightTemplateId,
                MinOrderSubtotal = x.MinOrderSubtotal,
                ShippingPrice = x.ShippingPrice,
                StateOrProvinceId = x.StateOrProvinceId,
                Note = x.Note,
                CountryName = x.Country.Name,
                StateOrProvinceName = x.StateOrProvince != null ? x.StateOrProvince.Name : null,
                StateOrProvinceLevel = x.StateOrProvince != null ? x.StateOrProvince.Level : level,
                CreatedOn = x.CreatedOn,
                UpdatedOn = x.UpdatedOn,
                IsEnabled = x.IsEnabled
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Create a new freight strategy according to the specified freight template.
    /// </summary>
    /// <param name="freightTemplateId">Freight template ID. </param>
    /// <param name="model">Build the parameters of the freight strategy. </param>
    /// <returns>The result of the create operation. </return>

    public async Task<Result> Post(int freightTemplateId, [FromBody] PriceAndDestinationCreateParam model)
    {
        var entity = new PriceAndDestination()
        {
            CountryId = model.CountryId,
            FreightTemplateId = freightTemplateId,
            MinOrderSubtotal = model.MinOrderSubtotal,
            StateOrProvinceId = model.StateOrProvinceId,
            ShippingPrice = model.ShippingPrice,
            Note = model.Note,
            IsEnabled = model.IsEnabled
        };
        var any = await _priceAndDestinationRepository.Query().AnyAsync(c =>
            c.FreightTemplateId == entity.FreightTemplateId
            && c.CountryId == model.CountryId
            && c.StateOrProvinceId == model.StateOrProvinceId);
        if (any)
            return Result.Fail("Freight policy already exists. There can only be one freight policy for the same country, province, city or city.");

        _priceAndDestinationRepository.Add(entity);
        await _priceAndDestinationRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Updates the shipping policy for the specified ID.
    /// </summary>
    /// <param name="id">Shipping policy ID. </param>
    /// <param name="model">Updates the freight strategy parameters. </param>
    /// <returns>The result of the update operation. </return>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] PriceAndDestinationCreateParam model)
    {
        var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
        if (entity == null)
            return Result.Fail("Documentation does not exist");

        var any = await _priceAndDestinationRepository.Query().AnyAsync(c =>
            c.FreightTemplateId == entity.FreightTemplateId
            && c.CountryId == model.CountryId
            && c.StateOrProvinceId == model.StateOrProvinceId
            && c.Id != entity.Id);
        if (any)
            return Result.Fail("Freight policy already exists. There can only be a freight policy for the same country, province, city or municipality.");

        entity.CountryId = model.CountryId;
        entity.MinOrderSubtotal = model.MinOrderSubtotal;
        entity.StateOrProvinceId = model.StateOrProvinceId;
        entity.ShippingPrice = model.ShippingPrice;
        entity.Note = model.Note;
        entity.IsEnabled = model.IsEnabled;
        entity.UpdatedOn = DateTime.Now;
        await _priceAndDestinationRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the shipping policy with the specified ID.
    /// </summary>
    /// <param name="id">Shipping policy ID. </param>
    /// <returns>Result of the delete operation. </return>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
        if (entity == null)
            return Result.Fail("Documentation does not exist");

        entity.IsDeleted = true;
        entity.UpdatedOn = DateTime.Now;
        await _priceAndDestinationRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
