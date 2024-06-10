namespace Soul.Shop.Module.Payment.Abstractions.ViewModels
{
    public class CashfreeConfigForm
    {
        public bool IsSandbox { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public string ReturnURL { get; set; }
        public string NotifyURL { get; set; }
        public string PaymentModes { get; set; }
    }
}
