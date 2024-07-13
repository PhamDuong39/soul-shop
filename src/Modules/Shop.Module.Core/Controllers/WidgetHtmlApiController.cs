using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Models;
using Shop.Module.Core.ViewModels;

namespace Shop.Module.Core.Controllers;

/// <summary>
/// Widget Html API controller, providing interface operations related to HTML widgets.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/widget-html")]
public class WidgetHtmlApiController : ControllerBase
{
    private readonly IRepository<WidgetInstance> _widgetInstanceRepository;

    public WidgetHtmlApiController(IRepository<WidgetInstance> widgetInstanceRepository)
    {
        _widgetInstanceRepository = widgetInstanceRepository;
    }

    /// <summary>
    /// Retrieve HTML widget information based on the widget instance ID
    /// </summary>
    /// <param name="id">widget instance ID</param>
    /// <returns>HTML widget information</returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widget = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widget == null) return Result.Fail("Document does not exist");
        var model = new WidgetHtmlResult
        {
            Id = widget.Id,
            Name = widget.Name,
            WidgetZoneId = widget.WidgetZoneId,
            HtmlContent = widget.HtmlData,
            PublishStart = widget.PublishStart,
            PublishEnd = widget.PublishEnd,
            DisplayOrder = widget.DisplayOrder
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Create a new HTML widget
    /// </summary>
    /// <param name="model">HTML widget parameters </param>
    /// <returns>widget parameters </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] WidgetHtmlParam model)
    {
        var widgetInstance = new WidgetInstance
        {
            Name = model.Name,
            WidgetId = (int)WidgetWithId.HtmlWidget,
            WidgetZoneId = model.WidgetZoneId,
            HtmlData = model.HtmlContent,
            PublishStart = model.PublishStart,
            PublishEnd = model.PublishEnd,
            DisplayOrder = model.DisplayOrder
        };
        _widgetInstanceRepository.Add(widgetInstance);
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update HTML widget information
    /// </summary>
    /// <param name="id">widget instance ID </param>
    /// <param name="model">HTML widget parameters </param>
    /// <returns>Operation result </returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetHtmlParam model)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null) return Result.Fail("Document does not exist");
        widgetInstance.Name = model.Name;
        widgetInstance.WidgetZoneId = model.WidgetZoneId;
        widgetInstance.HtmlData = model.HtmlContent;
        widgetInstance.PublishStart = model.PublishStart;
        widgetInstance.PublishEnd = model.PublishEnd;
        widgetInstance.DisplayOrder = model.DisplayOrder;
        widgetInstance.UpdatedOn = DateTime.Now;
        await _widgetInstanceRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
