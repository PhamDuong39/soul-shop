using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Module.Minio.Abstractions.Service;

namespace Soul.Shop.Module.Minio.ModuleRegister
{
    public static class ServiceCollection
    {
        public static IServiceCollection AddServiceCollection(this IServiceCollection services)
        {
            return services.AddTransient<IFileManagerService, FileManagerService>();
        }
    }
}
