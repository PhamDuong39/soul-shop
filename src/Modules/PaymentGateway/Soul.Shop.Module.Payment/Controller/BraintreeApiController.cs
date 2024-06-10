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
    [Area("PaymentBraintree")]
    [Authorize(Roles = "admin")]
    [Route("api/braintree")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BraintreeApiController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public BraintreeApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var stripeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.BraintreeProviderId);
            var model = JsonConvert.DeserializeObject<BraintreeConfigForm>(stripeProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] BraintreeConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var stripeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.BraintreeProviderId);
                stripeProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }
    }
}
