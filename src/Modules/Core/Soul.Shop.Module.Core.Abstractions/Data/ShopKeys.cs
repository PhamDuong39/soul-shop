namespace Soul.Shop.Module.Core.Abstractions.Data;

public class ShopKeys
{
    public static readonly string System = "shop";

    public static string RegisterPhonePrefix = System + ":register:phone:";

    public static string UserJwtTokenPrefix = System + ":user:jwt:token:";

    public static string Provinces = System + ":country:";


    public const string GuestDefaultPassword = "123456";

    public const string UserGuidCookiesName = "ShopUserGuid";
}