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

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// The management backend controller is used to handle API requests for simple product widget related operations.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/widget-simple-products")]
public class WidgetSimpleProductApiController : ControllerBase
{
    private readonly IRepository<WidgetInstance> _widgetInstanceRepository;
    private readonly IRepository<Product> _productRepository;

    public WidgetSimpleProductApiController(
        IRepository<WidgetInstance> widgetInstanceRepository,
        IRepository<Product> productRepository)
    {
        _widgetInstanceRepository = widgetInstanceRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Get the simple product widget information according to the specified widget instance ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <returns>The <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null)
            return Result.Fail("The document does not exist");
        var model = new WidgetSimpleProductResult
        {
            Id = widgetInstance.Id,
            Name = widgetInstance.Name,
            WidgetZoneId = widgetInstance.WidgetZoneId,
            PublishStart = widgetInstance.PublishStart,
            PublishEnd = widgetInstance.PublishEnd,
            DisplayOrder = widgetInstance.DisplayOrder,
            Setting = JsonConvert.DeserializeObject<WidgetSimpleProductSetting>(widgetInstance.Data)
        };
        if (model.Setting == null) model.Setting = new WidgetSimpleProductSetting();
        if (model.Setting?.Products?.Count > 0)
        {
            // Verify the publishing status
            var productIds = model.Setting.Products.Select(c => c.Id).Distinct();
            model.Setting.Products = await _productRepository.Query().Where(c => productIds.Contains(c.Id)).Select(c =>
                new ProductLinkResult()
                {
                    Id = c.Id,
                    IsPublished = c.IsPublished,
                    Name = c.Name
                }).ToListAsync();
        }

        return Result.Ok(model);
    }

    /// <summary>
    /// Create a new simple product widget.
    /// </summary>
    /// <param name="model">Simple product widget parameters to be created. </param>
    /// <returns>A <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WidgetSimpleProductParam model)
    {
        var widgetInstance = new WidgetInstance
        {
            Name = model.Name,
            WidgetId = (int)WidgetWithId.SimpleProductWidget,
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
    /// Update the simple product widget information of the specified ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <param name="model">Updated simple product widget parameters. </param>
    /// <returns>The <see cref="Result"/> object indicating the result of the operation. </returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetSimpleProductParam model)
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
}
