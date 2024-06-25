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
/// Bộ điều khiển API mẫu vận chuyển hàng hóa, chịu trách nhiệm quản lý các hoạt động liên quan đến mẫu vận chuyển hàng hóa.
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
    /// Nhận danh sách tất cả các mẫu vận chuyển.
    /// </summary>
    /// <returns>Danh sách các mẫu vận chuyển.</returns>
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
    /// Nhận danh sách các mẫu vận chuyển hàng hóa trong các trang dựa trên các thông số đã cho.
    /// </summary>
    /// <param name="param">Các tham số phân trang và truy vấn. </param>
    /// <returns>Danh sách phân trang của các mẫu vận chuyển.</returns> 
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
    /// Tạo mẫu giá vận chuyển mới.
    /// </summary>
    /// <param name="model">Tạo các thông số của mẫu vận chuyển hàng hóa. </param>
    /// <returns>Tạo kết quả của hoạt động. </returns> 
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
    /// Cập nhật mẫu vận chuyển được chỉ định.
    /// </summary>
    /// <param name="id">ID của mẫu vận chuyển cần cập nhật.</param>
    /// <param name="model">Cập nhật thông số cho mẫu cước vận chuyển.</param>
    /// <returns>Kết quả của hoạt động cập nhật. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] FreightTemplateCreateParam model)
    {
        var template = await _freightTemplateRepository.FirstOrDefaultAsync(id);
        if (template == null)
            return Result.Fail("运费模板不存在");
        template.Name = model.Name;
        template.Note = model.Note;
        template.UpdatedOn = DateTime.Now;
        await _freightTemplateRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Xóa mẫu vận chuyển hàng hóa được chỉ định.
    /// </summary>
    /// <param name="id">ID của mẫu vận chuyển sẽ bị xóa. </param>
    /// <returns> Kết quả của thao tác xóa. </returns>
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
