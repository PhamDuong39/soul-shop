using Braintree;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Payment.Abstractions.Entities;
using Soul.Shop.Module.Payment.Abstractions.Helper;
using Soul.Shop.Module.Payment.Abstractions.ViewModels;

namespace Soul.Shop.Module.Payment.Service
{
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        public string Environment { get; private set; }
        public string MerchantId { get; private set; }
        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }

        public async Task<IBraintreeGateway> BraintreeGateway()
        {
            if (_braintreeGateway == null)
            {
                _braintreeGateway = await CreateGateway();
            }

            return _braintreeGateway;
        }

        private IBraintreeGateway _braintreeGateway { get; set; }

        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public BraintreeConfiguration(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }


        private async Task<IBraintreeGateway> CreateGateway()
        {
            var braintreeProvider = await _paymentProviderRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.BraintreeProviderId);
            var braintreeSetting =
                JsonConvert.DeserializeObject<BraintreeConfigForm>(braintreeProvider.AdditionalSettings);

            Environment = braintreeSetting.Environment;
            MerchantId = braintreeSetting.MerchantId;
            PublicKey = braintreeSetting.PublicKey;
            PrivateKey = braintreeSetting.PrivateKey;

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        public async Task<string> GetClientToken()
        {
            var gateway = await BraintreeGateway();
            return await gateway.ClientToken.GenerateAsync();
        }
    }
}
