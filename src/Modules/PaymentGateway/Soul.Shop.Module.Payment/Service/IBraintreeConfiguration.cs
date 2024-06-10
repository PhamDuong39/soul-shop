using Braintree;

namespace Soul.Shop.Module.Payment.Service
{
    public interface IBraintreeConfiguration
    {
        string Environment { get; }

        string MerchantId { get; }

        string PublicKey { get; }

        string PrivateKey { get; }

        Task<IBraintreeGateway> BraintreeGateway();

        Task<string> GetClientToken();
    }
}
