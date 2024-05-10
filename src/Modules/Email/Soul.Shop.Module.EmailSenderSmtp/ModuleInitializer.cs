using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Core.Abstractions.Services;

namespace Soul.Shop.Module.EmailSenderSmtp;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailSender>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}