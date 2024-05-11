using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Orders.Abstractions.Events;
using Soul.Shop.Module.Orders.Handlers;

namespace Soul.Shop.Module.Orders;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //Using for register services
        // services.AddTransient<IOrderService, OrderService>();

        //services.AddTransient<IOrderEmailService, OrderEmailService>();
        //services.AddHostedService<OrderCancellationBackgroundService>();

        services.AddTransient<INotificationHandler<OrderChanged>, OrderChangedCreateOrderHistoryHandler>();
        services.AddTransient<INotificationHandler<OrderCreated>, OrderCreatedCreateOrderHistoryHandler>();
        services.AddTransient<INotificationHandler<PaymentReceived>, PaymentReceivedHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}