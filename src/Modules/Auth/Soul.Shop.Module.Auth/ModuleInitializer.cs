using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;

namespace Soul.Shop.Module.Auth;

public class ModuleInitializer : IModuleInitializer
{
    private const string IpRateLimitingKey = "IpRateLimitingEnabled";
    private const string ClientRateLimitingKey = "ClientRateLimitingEnabled";

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var ipRate = configuration.GetSection(IpRateLimitingKey).Get<bool>();
        var clientRate = configuration.GetSection(ClientRateLimitingKey).Get<bool>();
        if (ipRate || clientRate) services.AddRateLimit(configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var conf = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var ipRate = conf.GetSection(IpRateLimitingKey).Get<bool>();
        var clientRate = conf.GetSection(ClientRateLimitingKey).Get<bool>();

        if (ipRate) app.UseIpRateLimit();

        if (clientRate) app.UseClientRateLimit();
    }
}