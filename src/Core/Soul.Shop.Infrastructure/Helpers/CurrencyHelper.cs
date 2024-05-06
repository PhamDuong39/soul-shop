using System.Globalization;

namespace Soul.Shop.Infrastructure.Helpers;

public static class CurrencyHelper
{
    private static readonly List<string> ZeroDecimalCurrencies =
        ["BIF", "DJF", "JPY", "KRW", "PYG", "VND", "XAF", "XPF", "CLP", "GNF", "KMF", "MGA", "RWF", "VUV", "XOF"];

    public static bool IsZeroDecimalCurrencies()
    {
        var regionInfo = new RegionInfo(CultureInfo.CurrentCulture.LCID);
        return ZeroDecimalCurrencies.Contains(regionInfo.ISOCurrencySymbol);
    }
}