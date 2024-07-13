using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// The management backend controller is used to process API requests for widget product related operations.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/widget-products")]
public class WidgetProductApiController : ControllerBase
{
    private readonly IRepository<WidgetInstance> _widgetInstanceRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IMediaService _mediaService;

    public WidgetProductApiController(
        IRepository<WidgetInstance> widgetInstanceRepository,
        IRepository<Product> productRepository,
        IMediaService mediaService)
    {
        _widgetInstanceRepository = widgetInstanceRepository;
        _productRepository = productRepository;
        _mediaService = mediaService;
    }

    /// <summary>
    /// Get widget product information based on the specified widget instance ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <returns>The <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null)
            return Result.Fail("The document does not exist");
        var model = new WidgetProductResult
        {
            Id = widgetInstance.Id,
            Name = widgetInstance.Name,
            WidgetZoneId = widgetInstance.WidgetZoneId,
            PublishStart = widgetInstance.PublishStart,
            PublishEnd = widgetInstance.PublishEnd,
            DisplayOrder = widgetInstance.DisplayOrder,
            Setting = JsonConvert.DeserializeObject<WidgetProductSetting>(widgetInstance.Data)
        };
        var enumMetaData = MetadataProvider.GetMetadataForType(typeof(WidgetProductOrderBy));
        return Result.Ok(model);
    }

    /// <summary>
    /// Create a new widget product.
    /// </summary>
    /// <param name="model">Widget product parameters to be created. </param>
    /// <returns>A <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WidgetProductParam model)
    {
        var widgetInstance = new WidgetInstance
        {
            Name = model.Name,
            WidgetId = (int)WidgetWithId.ProductWidget,
            WidgetZoneId = model.WidgetZoneId,
            PublishStart = model.PublishStart,
            PublishEnd = model.PublishEnd,
            DisplayOrder = model.DisplayOrder,
            Data = JsonConvert.SerializeObject(model.Setting)
        };
        _widgetInstanceRepository.Add(widgetInstance);
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the widget product information of the specified ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <param name="model">Updated widget product parameters. </param>
    /// <returns>The <see cref="Result"/> object indicating the result of the operation. </returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetProductParam model)
    {
        var widgetInstance = _widgetInstanceRepository.Query().FirstOrDefault(x => x.Id == id);
        if (widgetInstance == null)
            return Result.Fail("The document does not exist");
        widgetInstance.Name = model.Name;
        widgetInstance.WidgetZoneId = model.WidgetZoneId;
        widgetInstance.PublishStart = model.PublishStart;
        widgetInstance.PublishEnd = model.PublishEnd;
        widgetInstance.DisplayOrder = model.DisplayOrder;
        widgetInstance.Data = JsonConvert.SerializeObject(model.Setting);
        widgetInstance.UpdatedOn = DateTime.Now;
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok();
    }

    //[HttpGet("available-orderby")]
    //public async Task<Result> GetAvailableOrderBy()
    //{
    //    var model = EnumHelper.ToDictionary(typeof(WidgetProductOrderBy)).Select(x => new { Id = x.Key.ToString(), Name = x.Value });
    //    return Result.Ok(model);
    //}
}
