using Soul.Shop.Module.Core.Abstractions.Data;

namespace Soul.Shop.Module.Catalog.Abstractions.Data;

public class CatalogKeys : ShopKeys
{
    public static string Module = System + ":catalog";


    public static string UnitAll = Module + ":unit:all";


    public static string BrandAll = Module + ":brand:all";


    public static string CategoryAll = Module + ":category:all";


    public static string AttributeAll = Module + ":attribute:all";


    public static string TemplateAll = Module + ":template:all";


    public static string GoodsById = Module + ":goods:";
}