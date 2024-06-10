using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace Soul.Shop.Module.Payment.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IConfiguration _config;

        public CurrencyService(IConfiguration config)
        {
            _config = config;
            var currencyCulture = _config.GetValue<string>("Global.CurrencyCulture");
            CurrencyCulture = new CultureInfo(currencyCulture);
        }

        public CultureInfo CurrencyCulture { get; }

        public string FormatCurrency(decimal value)
        {
            var decimalPlace = _config.GetValue<int>("Global.CurrencyDecimalPlace");
            return value.ToString($"C{decimalPlace}", CurrencyCulture);
        }
    }
}
