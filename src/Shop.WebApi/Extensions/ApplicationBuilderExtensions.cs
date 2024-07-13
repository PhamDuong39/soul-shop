using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shop.Infrastructure.Modules;

namespace Shop.WebApi.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseCustomizedConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Tên miền chéo
        app.UseCors(builder =>
        {
            builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
        });

        // Tài nguyên tĩnh
        app.UseStaticFiles();

        // mô-đun
        var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
        foreach (var moduleInitializer in moduleInitializers) moduleInitializer.Configure(app, env);
    }
}
