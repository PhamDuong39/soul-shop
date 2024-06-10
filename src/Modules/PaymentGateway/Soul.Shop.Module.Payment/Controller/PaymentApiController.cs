using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Service;

namespace Soul.Shop.Module.Payment.Controller
{
    [Area("Payments")]
    [Authorize(Roles = "admin")]
    [Route("api/payments")]
    public class PaymentApiController(
        IRepository<Abstractions.Entities.Payment> paymentRepository,
        ICurrencyService currencyService)
        : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ICurrencyService _currencyService = currencyService;

        [HttpGet("/api/orders/{orderId}/payments")]
        public async Task<IActionResult> GetByOrder(long orderId)
        {
            var payments = await paymentRepository.Query()
                .Where(x => x.OrderId == orderId)
                .Select(x => new
                {
                    x.Id,
                    x.Amount,
                    AmountString = _currencyService.FormatCurrency(x.Amount),
                    x.PaymentFee,
                    PaymentFeeString = _currencyService.FormatCurrency(x.PaymentFee),
                    x.OrderId,
                    x.GatewayTransactionId,
                    Status = x.Status.ToString(),
                    x.CreatedOn
                }).ToListAsync();

            return Ok(payments);
        }
    }
}
