using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Core.Abstractions.Services;
using Soul.Shop.Module.StorageGitHub.Services;

namespace Soul.Shop.Module.StorageGitHub;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStorageService, GitHubStorageService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
