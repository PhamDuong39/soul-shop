using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.SampleData.Data;
using Soul.Shop.Module.SampleData.Services;

namespace Soul.Shop.Module.SampleData;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ISqlRepository, SqlRepository>();
        services.AddTransient<ISampleDataService, SampleDataService>();
        services.AddTransient<IStateOrProvinceService, StateOrProvinceService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}