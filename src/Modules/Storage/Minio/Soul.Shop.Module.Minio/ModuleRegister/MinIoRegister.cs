using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Soul.Shop.Module.Minio.Abstractions.Options;

namespace Soul.Shop.Module.Minio.ModuleRegister;

public static class MinIoRegister
{
    public static IServiceCollection InitializeMinIo(this IServiceCollection service, IConfiguration configuration)
    {
        var minIoConfig = configuration.GetSection(MinIoConfigOption.Position).Get<MinIoConfigOption>();
        if (minIoConfig == null)
        {
            Console.WriteLine("=======Has not define Min IO configuration yet!!!=======");
            throw new Exception("=======Has not define Min IO configuration yet!!!=======");
        }
        //oldest - not support default bucket and default location

        // return service.AddMinio(config => config
        //     .WithEndpoint(minIoConfig.EndPoint)
        //     .WithCredentials(minIoConfig.AccessKey, minIoConfig.SecretKey)
        //     .WithSSL(minIoConfig.Secure));

        //newest - support default bucket and default location
        service.AddSingleton<MinioClient>(
            _ =>(MinioClient) new MinioClient()
                .WithEndpoint(minIoConfig.EndPoint)
                .WithCredentials(minIoConfig.AccessKey, minIoConfig.SecretKey)
                .WithSSL(minIoConfig.Secure)
                .WithRegion(minIoConfig.DefaultLocation)
                .Build()
        );
        return service;
    }
}
