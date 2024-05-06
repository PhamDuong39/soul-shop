using Serilog;
using Soul.Shop.Api.Extension;


try
{
    var builder = WebApplication.CreateBuilder(args);
    // load up serilog configuraton
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    builder.Host.UseSerilog(Log.Logger);

    Log.Information("Application startup services registration");

    builder.Services.AddSwaggerExtension();
    builder.Services.AddControllersExtension();
    builder.Services.AddCorsExtension();
    builder.Services.AddHealthChecks();
    builder.Services.AddJWTAuthentication(builder.Configuration);
    builder.Services.AddAuthorizationPolicies(builder.Configuration);
    var app = builder.Build();

    Log.Information("Application startup middleware registration");

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSwaggerExtension();
    app.UseHealthChecks("/health");
    app.MapControllers();

    Log.Information("Application Starting");

    app.Run();
}
catch (Exception ex)
{
    Log.Warning(ex, "An error occurred starting the application");
}
finally
{
    Log.CloseAndFlush();
}