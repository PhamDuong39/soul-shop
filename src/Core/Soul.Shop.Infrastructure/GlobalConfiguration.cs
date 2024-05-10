using Microsoft.Extensions.Configuration;
using Soul.Shop.Infrastructure.Localization;
using Soul.Shop.Infrastructure.Modules;

namespace Soul.Shop.Infrastructure;

public static class GlobalConfiguration
{
    public static DateTime InitialOn = new(2019, 1, 1, 0, 0, 0, DateTimeKind.Local);

    public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();

    public static IList<Culture> Cultures { get; set; } = new List<Culture>();

    public static string DefaultCulture => "en-US";

    public static string WebRootPath { get; set; }

    public static string ContentRootPath { get; set; }

    public static IConfiguration Configuration { get; set; }

    public const string NoImage = "no-image.png";
}