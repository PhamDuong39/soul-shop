using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;
using Soul.Shop.Module.Payment.Abstractions.Helper;
using Soul.Shop.Module.Payment.Abstractions.ViewModels;

namespace Soul.Shop.Module.Payment.Controller
{
    [Area("PaymentCashfree")]
    [Authorize(Roles = "admin")]
    [Route("api/cashfree")]
    public class CashfreeApiController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public CashfreeApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var cashfreeProvider = await _paymentProviderRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.CashfreeProviderId);
            var model = JsonConvert.DeserializeObject<CashfreeConfigForm>(cashfreeProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] CashfreeConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var cashfreeProvider = await _paymentProviderRepository.Query()
                    .FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.CashfreeProviderId);
                cashfreeProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }
    }
}
