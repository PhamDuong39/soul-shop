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
/// The management backend controller is used to handle API requests for recently browsed widget related operations.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/widget-recently-viewed")]
public class WidgetRecentlyViewedApiController : ControllerBase
{
    private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

    public WidgetRecentlyViewedApiController(
        IRepository<WidgetInstance> widgetInstanceRepository)
    {
        _widgetInstanceRepository = widgetInstanceRepository;
    }

    /// <summary>
    /// Get the most recently viewed widget information based on the specified widget instance ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <returns>The <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null) return Result.Fail("The document does not exist");
        var model = new WidgetRecentlyViewedResult
        {
            Id = widgetInstance.Id,
            Name = widgetInstance.Name,
            WidgetZoneId = widgetInstance.WidgetZoneId,
            PublishStart = widgetInstance.PublishStart,
            PublishEnd = widgetInstance.PublishEnd,
            DisplayOrder = widgetInstance.DisplayOrder,
            ItemCount = JsonConvert.DeserializeObject<int>(widgetInstance.Data)
        };
        return Result.Ok(model);
    }


    /// <summary>
    /// Create a new recently browsed widget.
    /// </summary>
    /// <param name="model">Parameters of the recently browsed widget to be created. </param>
    /// <returns>The <see cref="Result"/> object representing the result of the operation. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WidgetRecentlyViewedParam model)
    {
        var widgetInstance = new WidgetInstance
        {
            Name = model.Name,
            WidgetId = (int)WidgetWithId.RecentlyViewedWidget,
            WidgetZoneId = model.WidgetZoneId,
            Data = model.ItemCount.ToString(),
            PublishStart = model.PublishStart,
            PublishEnd = model.PublishEnd,
            DisplayOrder = model.DisplayOrder
        };
        _widgetInstanceRepository.Add(widgetInstance);
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok(model);
    }

    /// <summary>
    /// Update the recently viewed widget information of the specified ID.
    /// </summary>
    /// <param name="id">Widget instance ID. </param>
    /// <param name="model">Updated recently viewed widget parameters. </param>
    /// <returns>The <see cref="Result"/> object indicating the result of the operation. </returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetRecentlyViewedParam model)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null) return Result.Fail("The document does not exist");
        widgetInstance.Name = model.Name;
        widgetInstance.PublishStart = model.PublishStart;
        widgetInstance.PublishEnd = model.PublishEnd;
        widgetInstance.WidgetZoneId = model.WidgetZoneId;
        widgetInstance.DisplayOrder = model.DisplayOrder;
        widgetInstance.Data = model.ItemCount.ToString();
        widgetInstance.UpdatedOn = DateTime.Now;
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
