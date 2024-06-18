﻿using Microsoft.AspNetCore.Authorization;
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
/// 管理后台控制器用于处理最近浏览小部件相关操作的 API 请求。
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
    /// 根据指定的小部件实例 ID 获取最近浏览小部件信息。
    /// </summary>
    /// <param name="id">小部件实例 ID。</param>
    /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
    [HttpGet("{id}")]
    public async Task<Result> Get(int id)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null) return Result.Fail("单据不存在");
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
    /// 创建一个新的最近浏览小部件。
    /// </summary>
    /// <param name="model">要创建的最近浏览小部件参数。</param>
    /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
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
    /// 更新指定 ID 的最近浏览小部件信息。
    /// </summary>
    /// <param name="id">小部件实例 ID。</param>
    /// <param name="model">更新后的最近浏览小部件参数。</param>
    /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
    [HttpPut("{id}")]
    public async Task<Result> Put(int id, [FromBody] WidgetRecentlyViewedParam model)
    {
        var widgetInstance = await _widgetInstanceRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
        if (widgetInstance == null) return Result.Fail("单据不存在");
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