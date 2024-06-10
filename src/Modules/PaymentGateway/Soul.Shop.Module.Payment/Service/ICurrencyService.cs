using System.Globalization;

namespace Soul.Shop.Module.Payment.Service
{
    public interface ICurrencyService
    {
        CultureInfo CurrencyCulture { get; }

        string FormatCurrency(decimal value);
    }
}
