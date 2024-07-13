using Shop.Module.Core.Data;

namespace Shop.Module.Catalog.Data;

public class CatalogKeys : ShopKeys
{
    public static string Module = System + ":catalog";

    /// <summary>
    /// Unit cache
    /// </summary>
    public static string UnitAll = Module + ":unit:all";

    /// <summary>
    /// Brand cache
    /// </summary>
    public static string BrandAll = Module + ":brand:all";

    /// <summary>
    /// Category cache
    /// </summary>
    public static string CategoryAll = Module + ":category:all";

    /// <summary>
    /// Product attribute cache
    /// </summary>
    public static string AttributeAll = Module + ":attribute:all";

    /// <summary>
    /// Product attribute template cache
    /// </summary>
    public static string TemplateAll = Module + ":template:all";

    /// <summary>
    /// Product information cache
    /// </summary>
    public static string GoodsById = Module + ":goods:";
}
