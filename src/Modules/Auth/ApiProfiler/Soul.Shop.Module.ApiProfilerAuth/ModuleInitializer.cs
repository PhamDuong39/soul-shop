using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.ApiProfiler;
using Soul.Shop.Module.Auth;
using Soul.Shop.Module.Auth.Dashboard;

namespace Soul.Shop.Module.ApiProfilerAuth;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetSection("ApiProfilerEnabled").Get<bool>())
        {
            services.Configure<ApiProfilerAuthOptions>(configuration.GetSection(nameof(ApiProfilerAuthOptions)));
            services.AddCsaApiProfiler();
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        if (configuration.GetSection("ApiProfilerEnabled").Get<bool>())
        {
            var miniOpts = app.ApplicationServices.GetRequiredService<IOptions<MiniProfilerOptions>>();
            var auth = app.ApplicationServices.GetRequiredService<IOptions<ApiProfilerAuthOptions>>();

            if (!string.IsNullOrWhiteSpace(miniOpts?.Value?.RouteBasePath)
                && !string.IsNullOrWhiteSpace(auth?.Value?.Username)
                && !string.IsNullOrWhiteSpace(auth?.Value?.Password))
            {
                var filterOptions = new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                            { Login = auth?.Value?.Username, PasswordClear = auth?.Value?.Password }
                    }
                };
                var options = new DashboardOptions
                {
                    Authorization = new[] { new BasicAuthAuthorizationFilter(filterOptions) }
                };

                //Must be executed before middleware that requires control
                app.UseWhen(
                    context => context.Request.Path.StartsWithSegments(new PathString(miniOpts.Value.RouteBasePath)),
                    x => x.UseMiddleware<AspNetCoreDashboardMiddleware>(options));
            }

            app.UseCsaApiProfiler();
        }
    }
}