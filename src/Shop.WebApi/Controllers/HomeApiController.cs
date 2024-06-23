﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Models;
using Shop.Module.Core.Services;
using Shop.Module.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers;

/// <summary>
/// 首页 API 控制器，提供用于处理首页相关内容的 API 接口。
/// </summary>
[ApiController]
[Route("api")]
public class HomeApiController(
    ILoggerFactory factory,
    IWidgetInstanceService widgetInstanceService,
    IRepository<Category> categoriesRepository,
    IMediaService mediaService,
    IRepository<Product> productRepository,
    IProductPricingService productPricingService,
    ICategoryService categoryService,
    IRepository<ProductCategory> productCategoryRepository)
{
    private readonly ILogger _logger = factory.CreateLogger("Unhandled Error");
    private readonly IProductPricingService _productPricingService = productPricingService;

    /// <summary>
    /// 获取发布的小部件实例集合。
    /// </summary>
    /// <returns>包含首页小部件实例的结果。</returns>
    [HttpGet("widgets")]
    public async Task<Result> Widgets()
    {
        var model = new HomeResult();
        model.WidgetInstances = await widgetInstanceService.GetPublished()
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new HomeWidgetInstanceResult
            {
                Id = x.Id,
                Name = x.Name,
                ViewComponentName = x.Widget.ViewComponentName,
                WidgetId = x.WidgetId,
                WidgetZoneId = x.WidgetZoneId,
                Data = x.Data,
                HtmlData = x.HtmlData,
                DisplayOrder = x.DisplayOrder
            }).ToListAsync();
        return Result.Ok(model);
    }

    /// <summary>
    /// 根据小部件实例 ID 获取特定的小部件内容。
    /// </summary>
    /// <param name="id">小部件实例的 ID。</param>
    /// <returns>特定小部件的内容。</returns>
    [HttpGet("widgets/{id:int:min(1)}")]
    public async Task<Result> Component(int id)
    {
        var model = await widgetInstanceService.GetPublished().Where(c => c.Id == id).FirstOrDefaultAsync();
        var enumInts = (int[])Enum.GetValues(typeof(WidgetWithId));
        if (!enumInts.Contains(model.WidgetId))
            return Result.Ok();
        switch ((WidgetWithId)model.WidgetId)
        {
            case WidgetWithId.CategoryWidget:
            {
                var result = new WidgetCategoryComponentResult()
                {
                    Id = model.Id,
                    WidgetId = model.WidgetId,
                    WidgetZoneId = model.WidgetZoneId,
                    WidgetName = model.Name
                };
                var settings = JsonConvert.DeserializeObject<WidgetCategorySetting>(model.Data);
                if (settings != null)
                    result.Categorys = await categoriesRepository.Query()
                        .Include(c => c.Media)
                        .Where(c => settings.CategoryIds.Contains(c.Id))
                        .Select(c => new CategoryHomeResult()
                        {
                            Id = c.Id,
                            ParentId = c.ParentId,
                            Description = c.Description,
                            Name = c.Name,
                            Slug = c.Slug,
                            ThumbnailUrl = c.Media == null ? null : c.Media.Url
                        }).ToListAsync();
                return Result.Ok(result);
            }
            case WidgetWithId.ProductWidget:
            {
                var result = new WidgetProductComponentResult()
                {
                    Id = model.Id,
                    WidgetId = model.WidgetId,
                    WidgetZoneId = model.WidgetZoneId,
                    WidgetName = model.Name,
                    Setting = JsonConvert.DeserializeObject<WidgetProductSetting>(model.Data) ??
                              new WidgetProductSetting()
                };
                var query = productRepository.Query()
                    .Where(x => x.IsPublished && x.IsVisibleIndividually);
                if (result.Setting.CategoryId.HasValue && result.Setting.CategoryId.Value > 0)
                {
                    var ids = new List<int>();
                    ids.Add(result.Setting.CategoryId.Value);
                    var all = await categoryService.GetAll();
                    ids.AddRange(categoryService.GetChildrens(result.Setting.CategoryId.Value, all).Select(c => c.Id)
                        .Distinct());

                    //query = query.Where(x => x.Categories.Any(c => c.CategoryId == result.Setting.CategoryId.Value));
                    var subQuery = from c in query
                        join b in productCategoryRepository.Query() on c.Id equals b.ProductId
                        where ids.Contains(b.CategoryId)
                        select c.Id;
                    query = query.Where(c => subQuery.Distinct().Contains(c.Id));
                }

                if (result.Setting.FeaturedOnly) query = query.Where(x => x.IsFeatured);
                switch (result.Setting.OrderBy)
                {
                    case WidgetProductOrderBy.Newest:
                        query = query.OrderByDescending(c => c.CreatedOn);
                        break;
                    case WidgetProductOrderBy.BestSelling:
                        // 暂无销量
                        query = query.OrderByDescending(c => c.CreatedOn);
                        break;
                    case WidgetProductOrderBy.Discount:
                        // 暂无折扣
                        query = query.Where(c => c.SpecialPrice > 0).OrderByDescending(c => c.UpdatedOn);
                        break;
                    default:
                        query = query.OrderByDescending(c => c.CreatedOn);
                        break;
                }

                result.Products = query
                    .Include(x => x.ThumbnailImage)
                    //.OrderByDescending(x => x.CreatedOn)
                    .Take(result.Setting.ItemCount)
                    .Select(x => new GoodsListResult
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Slug = x.Slug,
                        Price = x.Price,
                        OldPrice = x.OldPrice,
                        SpecialPrice = x.SpecialPrice,
                        SpecialPriceStart = x.SpecialPriceStart,
                        SpecialPriceEnd = x.SpecialPriceEnd,
                        IsAllowToOrder = x.IsAllowToOrder,
                        ThumbnailUrl = x.ThumbnailImage.Url,
                        ReviewsCount = x.ReviewsCount,
                        RatingAverage = x.RatingAverage,
                        ShortDescription = x.ShortDescription,
                        IsPublished = x.IsPublished,
                        IsFeatured = x.IsFeatured
                    })
                    .ToList();
                return Result.Ok(result);
            }
            case WidgetWithId.SimpleProductWidget:
            {
                var result = new WidgetSimpleProductComponentResult()
                {
                    Id = model.Id,
                    WidgetId = model.WidgetId,
                    WidgetZoneId = model.WidgetZoneId,
                    WidgetName = model.Name,
                    Setting = JsonConvert.DeserializeObject<WidgetSimpleProductSetting>(model.Data)
                };
                var productIds = result.Setting?.Products?.Select(c => c.Id);
                if (productIds.Count() > 0)
                {
                    var products = await productRepository
                        .Query()
                        .Include(c => c.ThumbnailImage)
                        .Where(x => x.IsPublished && productIds.Contains(x.Id)).ToListAsync();
                    foreach (var item in products)
                    {
                        var product = result.Setting.Products.FirstOrDefault(c => c.Id == item.Id);
                        if (product != null)
                        {
                            var productThumbnail = GoodsListResult.FromProduct(item);
                            result.Products.Add(productThumbnail);
                        }
                    }
                }

                return Result.Ok(result);
            }
            case WidgetWithId.CarouselWidget:
            {
                var result = new WidgetCarouselComponentResult()
                {
                    Id = model.Id,
                    WidgetId = model.WidgetId,
                    WidgetZoneId = model.WidgetZoneId,
                    Items = JsonConvert.DeserializeObject<IList<WidgetCarouselItem>>(model.Data)
                };

                if (result.Items.Count > 0)
                {
                    var mediaIds = result.Items.Select(c => c.ImageId).Distinct();
                    //var medias = await _mediaRepository.Query().Where(c => mediaIds.Contains(c.Id)).ToListAsync();
                    foreach (var item in result.Items)
                        // medias.FirstOrDefault(c => c.Id == item.ImageId)?.Url;
                        item.ImageUrl = await mediaService.GetMediaUrl(Path.GetFileName(item.ImageUrl));
                }

                return Result.Ok(result);
            }
            //case WidgetWithId.SpaceBarWidget:
            //    break;
            case WidgetWithId.RecentlyViewedWidget:
                break;
            case WidgetWithId.HtmlWidget:
            default:
                break;
        }

        return Result.Ok();
    }
}