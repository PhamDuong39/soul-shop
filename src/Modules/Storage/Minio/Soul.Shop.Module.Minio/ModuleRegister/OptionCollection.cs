using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Module.Minio.Abstractions.Options;

namespace Soul.Shop.Module.Minio.ModuleRegister;

public static class OptionCollection
{
    public static IServiceCollection AddOptionCollection(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .Configure<LogOption>(option => configuration.GetSection(LogOption.Position).Bind(option))
            .Configure<MinIoConfigOption>(option =>
                configuration.GetSection(MinIoConfigOption.Position).Bind(option))
            .Configure<ConfigOption>(option => configuration.GetSection(ConfigOption.Position).Bind(option))
            .Configure<PreviewOption>(option => configuration.GetSection(PreviewOption.Position).Bind(option));
    }
}
