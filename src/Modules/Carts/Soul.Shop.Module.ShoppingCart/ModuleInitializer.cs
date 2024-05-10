using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Core.Abstractions.Events;
using Soul.Shop.Module.ShoppingCart.Handlers;


namespace Soul.Shop.Module.ShoppingCart;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //To do: Add your services
        //Cart service
        services.AddTransient<INotificationHandler<UserSignedIn>, UserSignedInHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}