using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Module.PaymentNganLuong.ViewModels;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;
using Soul.Shop.Module.Payment.Abstractions.Helper;

namespace Soul.Shop.Module.Payment.Controller
{
    [Area("PaymentNganLuong")]
    [Authorize(Roles = "admin")]
    [Route("api/ngan-luong")]
    public class NganLuongApiController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public NganLuongApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var nganLuongProvider = await _paymentProviderRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.NganLuongPaymentProviderId);
            var model = JsonConvert.DeserializeObject<NganLuongConfigForm>(nganLuongProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] NganLuongConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var nganLuongProvider = await _paymentProviderRepository.Query()
                    .FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.NganLuongPaymentProviderId);
                nganLuongProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }
    }
}
