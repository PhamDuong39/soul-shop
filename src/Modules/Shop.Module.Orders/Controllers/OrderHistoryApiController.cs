using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Orders.Entities;

namespace Shop.Module.Orders.Controllers;

/// <summary>
/// Order History API controller, used to manage and query order history.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/orders/history")]
public class OrderHistoryApiController : ControllerBase
{
    private readonly IRepository<OrderHistory> _orderHistoryRepository;

    public OrderHistoryApiController(IRepository<OrderHistory> orderHistoryRepository)
    {
        _orderHistoryRepository = orderHistoryRepository;
    }

    /// <summary>
    /// Get all the history records of the specified order.
    /// </summary>
    /// <param name="orderId">Order ID. </param>
    /// <returns>A list of history records of the specified order. </returns>
    [HttpGet("{orderId:int:min(1)}")]
    public async Task<Result> Get(int orderId)
    {
        var histories = await _orderHistoryRepository
            .Query()
            .Include(c => c.CreatedBy)
            .Where(x => x.OrderId == orderId)
            .Select(x => new
            {
                x.Id,
                x.OldStatus,
                x.NewStatus,
                x.CreatedById,
                CreatedByFullName = x.CreatedBy.FullName,
                x.Note,
                x.CreatedOn
            }).OrderByDescending(c => c.Id).ToListAsync();
        return Result.Ok(histories);
    }
}
