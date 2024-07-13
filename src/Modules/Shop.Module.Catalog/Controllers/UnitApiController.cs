using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Unit API controller, responsible for managing commodity units.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/units")]
public class UnitApiController : ControllerBase
{
    private readonly string _key = CatalogKeys.UnitAll;
    private readonly IRepository<Unit> _unitRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IStaticCacheManager _cache;

    public UnitApiController(
        IRepository<Unit> unitRepository,
        IRepository<Product> productRepository,
        IStaticCacheManager cache)
    {
        _unitRepository = unitRepository;
        _productRepository = productRepository;
        _cache = cache;
    }

    /// <summary>
    /// Get all unit information.
    /// </summary>
    /// <returns> List of all units. </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var list = await GetAllByCache();
        var result = list.OrderBy(c => c.Name);
        return Result.Ok(result);
    }

    /// <summary>
    /// Create a new unit.
    /// </summary>
    /// <param name="model">Parameter object containing the unit name. </param>
    /// <returns>Operation results. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] NameParam model)
    {
        _unitRepository.Add(new Unit { Name = model.Name });
        await _unitRepository.SaveChangesAsync();
        await ClearCache();
        return Result.Ok();
    }

    /// <summary>
    /// Update the unit name of the specified ID.
    /// </summary>
    /// <param name="id">The unit ID to be updated. </param>
    /// <param name="model">The parameter object containing the new unit name. </param>
    /// <returns>The operation result. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] NameParam model)
    {
        var unit = await _unitRepository.FirstOrDefaultAsync(id);
        if (unit == null)
            return Result.Fail("Unit does not exist");
        unit.Name = model.Name;
        unit.UpdatedOn = DateTime.Now;
        await _unitRepository.SaveChangesAsync();
        await ClearCache();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the unit with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the unit to be deleted. </param>
    /// <returns>The result of the operation. If the unit is already in use, deletion is not allowed. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var unit = await _unitRepository.FirstOrDefaultAsync(id);
        if (unit == null)
            return Result.Fail("Unit does not exist");

        var any = await _productRepository.Query().AnyAsync(c => c.UnitId == id);
        if (any)
            return Result.Fail("The unit is already in use, deletion is not allowed");

        unit.IsDeleted = true;
        unit.UpdatedOn = DateTime.Now;
        await _unitRepository.SaveChangesAsync();
        await ClearCache();
        return Result.Ok();
    }

    /// <summary>
    /// Clear the cache of all unit information.
    /// </summary>
    /// <returns>Operation results. </returns>
    [HttpPost("clear-cache")]
    public async Task<Result> ClearAllCache()
    {
        await ClearCache();
        return Result.Ok();
    }

    private async Task ClearCache()
    {
        await Task.Run(() => { _cache.Remove(_key); });
    }

    private async Task<IList<Unit>> GetAllByCache()
    {
        return await _cache.GetAsync(_key, async () => { return await _unitRepository.Query().ToListAsync(); });
    }
}
