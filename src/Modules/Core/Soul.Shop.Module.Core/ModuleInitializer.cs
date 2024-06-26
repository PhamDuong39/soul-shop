﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Extensions;
using Soul.Shop.Module.Core.Extensions;

namespace Soul.Shop.Module.Core;

public class ModuleInitializer : IModuleInitializer
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var config = new ShopOptions();
        var configSection = configuration.GetSection(nameof(ShopOptions));
        configSection.Bind(config);
        services.Configure<ShopOptions>(configSection);
        services.Configure<AuthenticationOptions>(configuration.GetSection(nameof(AuthenticationOptions)));
        //using
        // services.AddTransient<IEntityService, EntityService>();
        // services.AddTransient<ITokenService, TokenService>();
        // services.AddTransient<IAccountService, AccountService>();
        // services.AddTransient<IMediaService, MediaService>();
        // services.AddTransient<ICountryService, CountryService>();
        // services.AddTransient<IAppSettingService, AppSettingService>();
        // services.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
        // services.AddTransient<IUserAddressService, UserAddressService>();

        services.AddScoped<IWorkContext, WorkContext>();

        // serviceCollection.AddTransient<IThemeService, ThemeService>();
        // serviceCollection.AddTransient<IWidgetInstanceService, WidgetInstanceService>();
        // serviceCollection.AddScoped<SignInManager<User>, SimplSignInManager<User>>();
        // serviceCollection.AddSingleton<SettingDefinitionProvider>();
        // serviceCollection.AddScoped<ISettingService, SettingService>();

        services.AddScoped<SignInManager<User>>();
        // services.AddScoped<ShopSignInManager<User>>();
        // services.AddScoped<SignInManager<User>, ShopSignInManager<User>>();


        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        //services.AddHttpContextAccessor();

        // LocalCache is registered as transient as its implementation resolves IMemoryCache, thus
        // there is no state to keep in its instance.
        //services.AddTransient<IDistributedCache, MemoryDistributedCache>();
        services.AddDistributedMemoryCache();

        //services.AddSingleton<IMemoryCache, MemoryCache>();
        services.AddMemoryCache();
        // using
        // services.AddScoped<ICacheManager, PerRequestCacheManager>();

        // using
        // if (config.RedisCachingEnabled)
        // {
        //     services.AddSingleton<IRedisConnectionWrapper, RedisConnectionWrapper>()
        //         .AddSingleton<ILocker, RedisConnectionWrapper>();
        //     services.AddScoped<IStaticCacheManager, RedisCacheManager>();
        // }
        // else
        // {
        //     services.AddSingleton<IStaticCacheManager, MemoryCacheManager>()
        //         .AddSingleton<ILocker, MemoryCacheManager>();
        // }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}