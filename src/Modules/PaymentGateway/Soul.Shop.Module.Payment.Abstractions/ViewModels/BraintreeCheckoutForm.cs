namespace Soul.Shop.Module.Payment.Abstractions.ViewModels
{
    public class BraintreeCheckoutForm
    {
        public string ClientToken { get; set; }

        public decimal Amount { get; set; }

        public string ISOCurrencyCode { get; set; }

        public Guid CheckoutId { get; set; }
    }
}
