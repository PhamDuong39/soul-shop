using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Soul.Shop.Module.Payment.Abstractions.ViewModels
{
    public class BraintreeConfigForm
    {
        [Required]
        public string PublicKey { get; set; }

        [Required]
        public string PrivateKey { get; set; }

        [Required]
        public string MerchantId { get; set; }

        [Required]
        public bool IsProduction { get; set; } = false;

        [JsonIgnore]
        public string Environment { get { return IsProduction ? "production" : "sandbox"; } }
    }
}
