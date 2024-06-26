using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Inventory.Entities;
using Shop.Module.Inventory.ViewModels;

namespace Shop.Module.Inventory.Areas.Inventory.Controllers;

/// <summary>
/// Inventory history API controller to manage and query inventory change history.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/stocks-histories")]
public class StockHistoryApiController : ControllerBase
{
    private readonly IRepository<StockHistory> _stockHistoryRepository;

    public StockHistoryApiController(IRepository<StockHistory> stockHistoryRepository)
    {
        _stockHistoryRepository = stockHistoryRepository;
    }

    /// <summary>
    /// Query inventory history by page.
    /// </summary>
    /// <param name="param">Pagination query parameters, which can include warehouse ID and product ID filter conditions. </param>
    /// <returns>Paginated inventory history list. </return>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<StockHistoryQueryResult>>> List(
        [FromBody] StandardTableParam<StockHistoryQueryParam> param)
    {
        var query = _stockHistoryRepository.Query();
        var warehouseId = param?.Search?.WarehouseId;
        var productId = param?.Search?.ProductId;
        if (warehouseId.HasValue)
            query = query.Where(c => c.WarehouseId == warehouseId.Value);
        if (productId.HasValue)
            query = query.Where(c => c.ProductId == productId.Value);

        var result = await query.Include(c => c.Warehouse)
            .ToStandardTableResult(param, c => new StockHistoryQueryResult
            {
                Id = c.Id,
                WarehouseId = c.WarehouseId,
                AdjustedQuantity = c.AdjustedQuantity,
                CreatedById = c.CreatedById,
                CreatedOn = c.CreatedOn,
                Note = c.Note,
                ProductId = c.ProductId,
                StockQuantity = c.StockQuantity,
                WarehouseName = c.Warehouse.Name
            });
        return Result.Ok(result);
    }
}
