using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;

namespace Soul.Shop.Module.Payment.Controller
{
    [Area("Payments")]
    [Authorize(Roles = "admin")]
    [Route("api/payments-providers")]
    public class PaymentProviderApiController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public PaymentProviderApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepositor)
        {
            _paymentProviderRepository = paymentProviderRepositor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var providers = await _paymentProviderRepository.Query()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsEnabled,
                    x.ConfigureUrl
                }).ToListAsync();

            return Json(providers);
        }

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(string id)
        {
            var provider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            provider.IsEnabled = true;
            await _paymentProviderRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(string id)
        {
            var provider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            provider.IsEnabled = false;
            await _paymentProviderRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
