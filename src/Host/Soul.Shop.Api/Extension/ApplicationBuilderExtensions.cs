using Soul.Shop.Infrastructure.Modules;

namespace Soul.Shop.Api.Extension;

public static class ApplicationBuilderExtensions
{
    public static void UseCustomizedConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
        {
            builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
        });

        app.UseStaticFiles();

        var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
        foreach (var moduleInitializer in moduleInitializers) moduleInitializer.Configure(app, env);
    }
}
