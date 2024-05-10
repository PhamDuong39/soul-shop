using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Catalog.Handlers;
using Soul.Shop.Module.Core.Abstractions.Events;

namespace Soul.Shop.Module.Catalog;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // using before
        // services.AddTransient<ICategoryService, CategoryService>();
        // services.AddTransient<IBrandService, BrandService>();
        // services.AddTransient<IProductPricingService, ProductPricingService>();
        // services.AddTransient<IProductService, ProductService>();
        services.AddTransient<INotificationHandler<EntityViewed>, EntityViewedHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}