namespace Soul.Shop.Api.Extension;

public static class AppExtensions
{
    public static void UseSwaggerExtension(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchitecture.Onion.WebApi"); });
    }

    public static void UseCustomizedConfigure(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(builder =>
        {
            builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
        });

        app.UseStaticFiles();

        // var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
        // foreach (var moduleInitializer in moduleInitializers) moduleInitializer.Configure(app, env);
    }
}