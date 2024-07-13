using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// The management backend controller is used to handle API requests for widget category related operations.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/widget-categories")]
public class WidgetCategoryApiController : ControllerBase
{
    private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

    public WidgetCategoryApiController(IRepository<WidgetInstance> widgetInstanceRepository)
    {
        _widgetInstanceRepository = widgetInstanceRepository;
    }


    /// <summary>
    /// Get widget category information based on the specified widget instance ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <returns>The <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(c => c.Id == id);
        if (widgetInstance == null)
            return Result.Fail("The document does not exist");
        var model = new WidgetCategoryResult
        {
            Id = widgetInstance.Id,
            Name = widgetInstance.Name,
            WidgetZoneId = widgetInstance.WidgetZoneId,
            PublishStart = widgetInstance.PublishStart,
            PublishEnd = widgetInstance.PublishEnd,
            DisplayOrder = widgetInstance.DisplayOrder,
            Setting = JsonConvert.DeserializeObject<WidgetCategorySetting>(widgetInstance.Data ??
                                                                           JsonConvert.SerializeObject(
                                                                               new WidgetCategorySetting()))
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Create a new widget category.
    /// </summary>
    /// <param name="model">Widget category parameters to be created. </param>
    /// <returns><see cref="Result"/> object representing the result of the operation. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WidgetCategoryParam model)
    {
        var widgetInstance = new WidgetInstance
        {
            Name = model.Name,
            WidgetId = (int)WidgetWithId.CategoryWidget,
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
    /// Update the widget category information of the specified ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <param name="model">Updated widget category parameters. </param>
    /// <returns>The <see cref="Result"/> object indicating the result of the operation. </returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetCategoryParam model)
    {
        var widgetInstance = await _widgetInstanceRepository.FirstOrDefaultAsync(id);
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
