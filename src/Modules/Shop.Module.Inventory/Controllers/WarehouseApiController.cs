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
/// Bộ điều khiển API quản lý kho, cung cấp các chức năng như thêm, xóa, sửa, truy vấn kho
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
    /// Nhận thông tin ngắn gọn về tất cả các kho
    /// </summary>
    /// <returns>Trả về kết quả thao tác, bao gồm danh sách thông tin tóm tắt về kho</returns>
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
    /// Lấy danh sách dữ liệu kho dựa trên tham số phân trang
    /// </summary>
    /// <param name="param">Các tham số bảng tiêu chuẩn, bao gồm phân trang, sắp xếp và các thông tin khác</param>
    /// <returns>Trả về kết quả hoạt động, bao gồm danh sách dữ liệu kho được phân trang</returns>
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
    /// Lấy thông tin chi tiết kho dựa trên ID kho
    /// </summary>
    /// <param name="id">ID kho</param>
    /// <returns>Trả về kết quả hoạt động, bao gồm chi tiết kho của ID đã chỉ định</returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();
        var warehouse = await _warehouseRepository.Query().Include(w => w.Address).FirstOrDefaultAsync(w => w.Id == id);
        if (warehouse == null)
            throw new Exception("Kho không tồn tại");
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
    /// Tạo kho mới
    /// </summary>
    /// <param name="model">Thông số tạo kho</param>
    /// <returns>Trả về kết quả phép toán, cho biết việc tạo có thành công hay không</returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WarehouseCreateParam model)
    {
        var currentUser = await _workContext.GetCurrentUserAsync();

        //Xác minh xem tỉnh hoặc thành phố được chọn có thuộc quốc gia hay không
        var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
        if (province == null)
            throw new Exception("Tỉnh hoặc thành phố không tồn tại");
        if (province.CountryId != model.CountryId)
            throw new Exception("Tỉnh hoặc thành phố được chọn không thuộc quốc gia hiện được chọn");

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
    /// Cập nhật kho lưu trữ
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
            throw new Exception("Kho không tồn tại");

        //Xác minh xem tỉnh hoặc thành phố được chọn có thuộc quốc gia hay không
        var province = await _provinceRepository.FirstOrDefaultAsync(model.StateOrProvinceId);
        if (province == null)
            throw new Exception("Tỉnh hoặc thành phố không tồn tại");
        if (province.CountryId != model.CountryId)
            throw new Exception("Tỉnh hoặc thành phố được chọn không thuộc quốc gia hiện được chọn");

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
            return Result.Fail("Kho không tồn tại");

        //var any = _productRepository.Query().Any(c => c.DefaultWarehouseId == id);
        //if (any)
        //    return Result.Fail("Kho đã được sản phẩm tham chiếu và không được phép xóa.");

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
