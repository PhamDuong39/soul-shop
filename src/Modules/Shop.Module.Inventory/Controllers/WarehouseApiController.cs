using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.Module.Inventory.Areas.Inventory.ViewModels;
using Shop.Module.Inventory.Entities;
using Shop.Module.Inventory.ViewModels;

namespace Shop.Module.Inventory.Areas.Inventory.Controllers;

/// <summary>
/// The warehouse management API controller provides functions such as adding, deleting, editing, and querying the warehouse
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/warehouses")]
public class WarehouseApiController : ControllerBase
{
    private readonly IRepository<Warehouse> _warehouseRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IWorkContext _workContext;
    private readonly IRepository<StateOrProvince> _provinceRepository;
    private readonly IRepository<Product> _productRepository;

    public WarehouseApiController(
        IRepository<Warehouse> warehouseRepository,
        IWorkContext workContext,
        IRepository<Address> addressRepository,
        IRepository<StateOrProvince> provinceRepository,
        IRepository<Product> productRepository)
    {
        _warehouseRepository = warehouseRepository;
        _addressRepository = addressRepository;
        _workContext = workContext;
        _provinceRepository = provinceRepository;
        _productRepository = productRepository;
    }


    /// <summary>
    /// Get brief information about all warehouses
    /// </summary>
    /// <returns>Returns the results of the operation, including a list of summary information about the warehouse</returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var currentUser = await _workContext.GetCurrentUserAsync();
        var query = _warehouseRepository.Query();
        var warehouses = await query.Select(x => new
        {
            x.Id,
            x.Name
        }).ToListAsync();
        return Result.Ok(warehouses);
    }

    /// <summary>
    /// Get a list of warehouse data based on the pagination parameter
    /// </summary>
    /// <param name="param">Standard table parameters, including paging, sorting, and more</param>
    /// <returns>Returns the results of the operation, including a paginated list of warehouse data</returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<WarehouseQueryResult>>> DataList([FromBody] StandardTableParam param)
    {
        var query = _warehouseRepository.Query();
        var result = await query.Include(w => w.Address)
            .ToStandardTableResult(param, warehouse => new WarehouseQueryResult
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                AddressId = warehouse.Address.Id,
                ContactName = warehouse.Address.ContactName,
                AddressLine1 = warehouse.Address.AddressLine1,
                AddressLine2 = warehouse.Address.AddressLine2,
                Phone = warehouse.Address.Phone,
                StateOrProvinceId = warehouse.Address.StateOrProvinceId,
                CountryId = warehouse.Address.CountryId,
                City = warehouse.Address.City,
                ZipCode = warehouse.Address.ZipCode,
                AdminRemark = warehouse.AdminRemark
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Get warehouse details based on warehouse ID
    /// </summary>
    /// <param name="id">Repository ID</param>
    /// <returns>Returns the results of the operation, including inventory details of the specified ID</returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();
        var warehouse = await _warehouseRepository.Query().Include(w => w.Address).FirstOrDefaultAsync(w => w.Id == id);
        if (warehouse == null)
            throw new Exception("Warehouse does not exist");
        var address = warehouse.Address ?? new Address();
        var result = new WarehouseQueryResult
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            AddressId = address.Id,
            ContactName = address.ContactName,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            Phone = address.Phone,
            StateOrProvinceId = address.StateOrProvinceId,
            CountryId = address.CountryId,
            City = address.City,
            ZipCode = address.ZipCode,
            AdminRemark = warehouse.AdminRemark
        };
        return Result.Ok(result);
    }

    /// <summary>
    /// Create a new repository
    /// </summary>
    /// <param name="model">Repository creation parameters</param>
    /// <returns>Returns the result of the operation, indicating whether the creation was successful or not</returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WarehouseCreateParam model)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();

        //Verify whether the selected province or city is part of the country
        var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
        if (province == null)
            throw new Exception("The province or city does not exist");
        if (province.CountryId != model.CountryId)
            throw new Exception("The selected province or city is not part of the currently selected country");

        var address = new Address
        {
            ContactName = model.ContactName,
            AddressLine1 = model.AddressLine1,
            AddressLine2 = model.AddressLine2,
            Phone = model.Phone,
            StateOrProvinceId = model.StateOrProvinceId,
            CountryId = model.CountryId,
            City = model.City,
            ZipCode = model.ZipCode
        };
        var warehouse = new Warehouse
        {
            Name = model.Name,
            Address = address,
            AdminRemark = model.AdminRemark
        };
        _warehouseRepository.Add(warehouse);
        await _warehouseRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the repository
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] WarehouseCreateParam model)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();
        var warehouse = await _warehouseRepository.Query()
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (warehouse == null)
            throw new Exception("Repository does not exist");

        //Verify whether the selected province or city belongs to the country
        var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
        if (province == null)
            throw new Exception("The province or city does not exist");
        if (province.CountryId != model.CountryId)
            throw new Exception("The selected province or city is not part of the currently selected country");

        warehouse.Name = model.Name;
        warehouse.AdminRemark = model.AdminRemark;
        warehouse.UpdatedOn = DateTime.Now;
        if (warehouse.Address == null)
        {
            warehouse.Address = new Address();
            _addressRepository.Add(warehouse.Address);
        }

        warehouse.Address.ContactName = model.ContactName;
        warehouse.Address.Phone = model.Phone;
        warehouse.Address.ZipCode = model.ZipCode;
        warehouse.Address.StateOrProvinceId = model.StateOrProvinceId;
        warehouse.Address.CountryId = model.CountryId;
        warehouse.Address.City = model.City;
        warehouse.Address.AddressLine1 = model.AddressLine1;
        warehouse.Address.AddressLine2 = model.AddressLine2;
        warehouse.Address.UpdatedOn = DateTime.Now;
        await _warehouseRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Xóa kho
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();
        var warehouse = await _warehouseRepository.Query()
            .Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (warehouse == null)
            return Result.Fail("Repository does not exist");

        //var any = _productRepository.Query().Any(c => c.DefaultWarehouseId == id);
        //if (any)
        // return Result.Fail("The warehouse has been referenced by the product and cannot be deleted.");

        warehouse.IsDeleted = true;
        warehouse.UpdatedOn = DateTime.Now;
        if (warehouse.Address != null)
        {
            warehouse.Address.IsDeleted = true;
            warehouse.Address.UpdatedOn = DateTime.Now;
        }

        await _warehouseRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
