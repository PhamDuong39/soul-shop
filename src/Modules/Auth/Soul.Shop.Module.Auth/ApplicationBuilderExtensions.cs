using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;

namespace Soul.Shop.Module.Auth;

public static class ApplicationBuilderExtensions
{
    public static void UseIpRateLimit(this IApplicationBuilder app)
    {
        app.UseIpRateLimiting();
    }

    public static void UseClientRateLimit(this IApplicationBuilder app)
    {
        app.UseClientRateLimiting();
    }
}