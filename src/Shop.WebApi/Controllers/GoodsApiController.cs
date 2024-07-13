using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Services;
using Shop.Module.Catalog.ViewModels;
using Shop.Module.Core.Cache;
using Shop.Module.Core.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.WebApi.Controllers;

/// <summary>
/// Bộ điều khiển API sản phẩm cung cấp giao diện API liên quan đến sản phẩm.
/// </summary>
[ApiController]
[Route("api/goods")]
public class GoodsApiController
{
    private readonly IProductService _productService;
    private readonly IStaticCacheManager _staticCacheManager;

    /// <summary>
    /// Trình xây dựng, tiêm dịch vụ hàng hóa và trình quản lý bộ nhớ đệm tĩnh.
    /// </summary>
    /// <param name="productService">Giao diện dịch vụ sản phẩm.</param>
    /// <param name="staticCacheManager">Giao diện quản lý bộ đệm tĩnh</param>
    public GoodsApiController(
        IProductService productService,
        IStaticCacheManager staticCacheManager)
    {
        _productService = productService;
        _staticCacheManager = staticCacheManager;
    }

    /// <summary>
    /// Nhận chi tiết sản phẩm dựa trên ID sản phẩm.
    /// </summary>
    /// <param name="id">ID sản phẩm.</param>
    /// <returns>Thông tin chi tiết sản phẩm.</returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var result = await _productService.GetGoodsByCache(id);
        return Result.Ok(result);
    }

    /// <summary>
    /// Nhận danh sách các sản phẩm liên quan đến sản phẩm được chỉ định.
    /// </summary>
    /// <param name="id">ID sản phẩm.</param>
    /// <returns>Danh sách các sản phẩm liên quan.</returns>
    [HttpGet("related/{id:int:min(1)}")]
    public async Task<Result> Related(int id)
    {
        var result = await _productService.RelatedList(id);
        return Result.Ok(result);
    }

    /// <summary>
    /// Nhận thông tin tồn kho của sản phẩm được chỉ định.
    /// </summary>
    /// <param name="id">ID sản phẩm. </param>
    /// <returns>Thông tin tồn kho sản phẩm. </returns>
    [HttpGet("stocks/{id:int:min(1)}")]
    public async Task<Result> GoodsStocks(int id)
    {
        var result = await _productService.GetGoodsStocks(id);
        return Result.Ok(result);
    }

    /// <summary>
    /// Lấy danh sách sản phẩm dựa trên các tham số truy vấn và phân trang hỗ trợ.
    /// </summary>
    /// <param name="param">Tham số truy vấn danh sách sản phẩm. </param>
    /// <returns>Kết quả bảng tiêu chuẩn, chứa danh sách sản phẩm. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<GoodsListResult>>> Grid(
        [FromBody] StandardTableParam<GoodsListQueryParam> param)
    {
        var resultKeyword = await GetKeywordAsync();
        if (!string.IsNullOrWhiteSpace(param?.Search?.Name) &&
            !resultKeyword.HistoryKeywords.Any(c => c.Name == param?.Search?.Name))
        {
            resultKeyword.HistoryKeywords.Insert(0, new Keyword()
            {
                Name = param?.Search?.Name.Trim()
            });
            if (resultKeyword.HistoryKeywords.Count > 10)
                resultKeyword.HistoryKeywords = resultKeyword.HistoryKeywords.Take(10).ToList();
            var key = ShopKeys.System + "keywords";
            _staticCacheManager.Set(key, resultKeyword, 30);
        }

        var result = await _productService.HomeList(param);
        return Result.Ok(result);
    }

    /// <summary>
    /// Nhận các từ khóa phổ biến và lịch sử tìm kiếm.
    /// </summary>
    /// <returns>Kết quả từ khóa, bao gồm các từ khóa phổ biến và lịch sử tìm kiếm. </returns>
    [HttpGet("keywords")]
    public async Task<Result> Keywords()
    {
        var result = await GetKeywordAsync();
        return Result.Ok(result);
    }

    /// <summary>
    /// Xóa lịch sử tìm kiếm.
    /// </summary>
    /// <returns>Kết quả hoạt động. </returns>
    [HttpPost("keywords/clear-histories")]
    public async Task<Result> ClearHistoryKeywords()
    {
        var result = await GetKeywordAsync();
        result.HistoryKeywords.Clear();
        var key = ShopKeys.System + "keywords";
        _staticCacheManager.Set(key, result, 30);
        return Result.Ok();
    }

    /// <summary>
    /// Nhận kết quả từ khóa một cách không đồng bộ, bao gồm các từ khóa phổ biến và lịch sử tìm kiếm.
    /// </summary>
    /// <returns>Kết quả từ khóa. </returns>
    private async Task<KeywordResult> GetKeywordAsync()
    {
        var key = ShopKeys.System + "keywords";
        var result = await _staticCacheManager.GetAsync(key, async () =>
        {
            return await Task.Run(() =>
            {
                var kr = new KeywordResult();
                kr.HotKeywords.Add(new Keyword { Name = "1" });
                kr.HotKeywords.Add(new Keyword { Name = "2" });
                return kr;
            });
        });
        return result;
    }
}

/// <summary>
/// Thực thể từ khóa, được sử dụng cho chức năng tìm kiếm.
/// </summary>
public class Keyword
{
    public string Name { get; set; }
    public int Heat { get; set; }
}

/// <summary>
/// Thực thể kết quả từ khóa, bao gồm từ khóa mặc định, từ khóa lịch sử và từ khóa phổ biến.
/// </summary>
public class KeywordResult
{
    public Keyword DefaultKeyword { get; set; } = new() { Name = "Bài kiểm tra" };
    public IList<Keyword> HistoryKeywords { get; set; } = new List<Keyword>();
    public IList<Keyword> HotKeywords { get; set; } = new List<Keyword>();
}
