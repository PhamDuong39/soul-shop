using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Infrastructure.Helpers;

public static class EnumHelper
{
    public static IDictionary<Enum, string> ToDictionary(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var enumValues = Enum.GetValues(type);

        return enumValues.Cast<Enum>().ToDictionary(value => value, value => GetDisplayName(value));
    }

    private static string GetDisplayName(this Enum value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        var displayName = value.ToString();
        var fieldInfo = value.GetType().GetField(displayName);
        var attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

        if (attributes?.Length > 0)
        {
            displayName = attributes[0].Description;
        }
        else
        {
            var desAttributes =
                (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (desAttributes?.Length > 0)
                displayName = desAttributes[0].Description;
        }

        return displayName;
    }
}