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
/// Bộ điều khiển API giá cước và điểm đến chịu trách nhiệm quản lý các quy tắc vận chuyển hàng hóa cụ thể trong mẫu vận chuyển hàng hóa.
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
    /// Nhận danh sách chiến lược vận chuyển hàng hóa trong các trang dựa trên ID mẫu vận chuyển hàng hóa.
    /// </summary>
    /// <param name="freightTemplateId">ID mẫu vận chuyển hàng hóa. </param>
    /// <param name="param">Thông số phân trang. </param>
    /// <returns> Danh sách các chiến lược vận chuyển được đánh số trang. </return>
    
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
    /// Tạo chiến lược vận chuyển hàng hóa mới theo mẫu vận chuyển hàng hóa đã chỉ định.
    /// </summary>
    /// <param name="freightTemplateId">ID mẫu vận chuyển hàng hóa. </param>
    /// <param name="model">Xây dựng các tham số của chiến lược vận chuyển hàng hóa. </param>
    /// <returns>Kết quả của thao tác tạo. </return>
    
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
            return Result.Fail("Chính sách vận chuyển hàng hóa đã tồn tại. Chỉ có thể có một chính sách vận chuyển hàng hóa cho cùng một quốc gia, tỉnh, thành phố hoặc thành phố.");

        _priceAndDestinationRepository.Add(entity);
        await _priceAndDestinationRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Cập nhật chính sách vận chuyển cho ID được chỉ định.
    /// </summary>
    /// <param name="id">ID chính sách vận chuyển. </param>
    /// <param name="model">Cập nhật các thông số của chiến lược vận chuyển hàng hóa. </param>
    /// <returns>Kết quả của thao tác cập nhật. </return>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] PriceAndDestinationCreateParam model)
    {
        var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
        if (entity == null)
            return Result.Fail("Tài liệu không tồn tại");

        var any = await _priceAndDestinationRepository.Query().AnyAsync(c =>
            c.FreightTemplateId == entity.FreightTemplateId
            && c.CountryId == model.CountryId
            && c.StateOrProvinceId == model.StateOrProvinceId
            && c.Id != entity.Id);
        if (any)
            return Result.Fail("Chính sách vận chuyển hàng hóa đã tồn tại. Chỉ có thể có một chính sách vận chuyển hàng hóa cho cùng một quốc gia, tỉnh, thành phố hoặc thành phố.");

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
    /// Xóa chính sách vận chuyển với ID được chỉ định.
    /// </summary>
    /// <param name="id">ID chính sách vận chuyển. </param>
    /// <returns>Kết quả của thao tác xóa. </return>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var entity = await _priceAndDestinationRepository.FirstOrDefaultAsync(id);
        if (entity == null)
            return Result.Fail("Tài liệu không tồn tại");

        entity.IsDeleted = true;
        entity.UpdatedOn = DateTime.Now;
        await _priceAndDestinationRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
