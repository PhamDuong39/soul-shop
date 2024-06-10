using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Soul.Shop.Api.Filters;
using Soul.Shop.Api.Handlers;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Modules;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Core.Data;
using Soul.Shop.Module.Core.Extensions;

namespace Soul.Shop.Api.Extension;

public static class ServiceCollectionExtensions
{
    public static void AddSwaggerDoc(this IServiceCollection services, string title = "", string name = "v1",
        string version = "1.0.0")
    {
        var assemblyMame = Assembly.GetCallingAssembly().GetName().Name;
        if (string.IsNullOrWhiteSpace(title)) title = assemblyMame!;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(name, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description =
                    "SOUL-SHOP API DOCUMENTATION",
                Contact = new OpenApiContact { Name = "", Email = "", },
                License = new OpenApiLicense { Name = "MIT License", }
            });

            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme()
                {
                    Description = "Token: Bearer {Token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] { }
                }
            });

            //var modelXmls = new string[] { "", "", "" };
            //foreach (var xmlModel in modelXmls)
            //{
            //    var baseDirectory = AppContext.BaseDirectory;
            //    if (!File.Exists(Path.Combine(baseDirectory, xmlModel)))
            //    {
            //        baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            //    }

            //    var xmlPath2 = Path.Combine(baseDirectory, xmlModel);
            //    if (File.Exists(xmlPath2))
            //    {
            //        c.IncludeXmlComments(xmlPath2);
            //    }
            //}

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var assemblyName = assembly.GetName().Name;

                if (assemblyName.StartsWith("Soul.Shop.Module."))
                {
                    var xmlSubFile = $"{assemblyName}.xml";
                    var xmlSubPath = Path.Combine(AppContext.BaseDirectory, xmlSubFile);
                    if (File.Exists(xmlSubPath)) c.IncludeXmlComments(xmlSubPath, true);
                }
            }

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{assemblyMame}.xml");
            if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath, true);
        });
    }

    public static void AddCustomizedConfigureServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env)
    {
        if (string.IsNullOrWhiteSpace(env.WebRootPath))
            env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        GlobalConfiguration.WebRootPath = env.WebRootPath;
        GlobalConfiguration.ContentRootPath = env.ContentRootPath;
        GlobalConfiguration.Configuration = configuration;

        services.AddModules(configuration);
        services.AddCustomizedDataStore(configuration);
        services.AddCustomizedIdentity(configuration);

        services.AddHttpClient();
        services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));

        // why??
        // services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        services.AddTransient<IAuthorizationHandler, PermissionHandler>();

        var sp = services.BuildServiceProvider();
        var moduleInitializers = sp.GetServices<IModuleInitializer>();
        foreach (var moduleInitializer in moduleInitializers)
            moduleInitializer.ConfigureServices(services, configuration);

        services.AddMvc(options =>
        {
            options.Filters.Add<CustomActionFilterAttribute>();
            options.Filters.Add<CustomExceptionFilterAttribute>();
        });
        // .AddNewtonsoftJson(options =>
        // {
        //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //
        //     //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        //
        //     options.SerializerSettings.DateTimeZoneHandling =
        //         DateTimeZoneHandling.Local; // json to datetime 2019-02-26T22:34:13.000Z -> 2019-02-27 06:34:13
        //     options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        // }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            options.User.RequireUniqueEmail = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = false;
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = true;
        });

        // mediatR
        //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly));
    }

    public static void AddModules(this IServiceCollection services, IConfiguration configuration)
    {
        var modules = configuration.GetSection("Modules").Get<List<ModuleInfo>>();

        foreach (var module in modules)
        {
            GlobalConfiguration.Modules.Add(module);

            module.Assembly = Assembly.Load(new AssemblyName(module.Id));

            var moduleType = module.Assembly.GetTypes()
                .FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
            if (moduleType != null && moduleType != typeof(IModuleInitializer))
                services.AddSingleton(typeof(IModuleInitializer), moduleType);
        }
    }

    public static void AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<ShopDbContext>(options => options.UseCustomizedDataStore(configuration));
    }

    public static void UseCustomizedDataStore(this DbContextOptionsBuilder options, IConfiguration configuration)
    {
        // SQL Server
        //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Shop.WebApi"));

        //// MySql
        //options.UseMySql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Shop.WebApi"));

        // MySql
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var serverVersion = new MySqlServerVersion(new Version(5, 7, 34));

        options.UseMySql(connectionString, serverVersion, b => b.MigrationsAssembly("Soul.Shop.Api"));
    }

    public static void AddCustomizedIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddUserStore<ShopUserStore>()
            .AddRoleStore<ShopRoleStore>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                // 302
                // or [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = false, // in this case, we don't care about the token's expiration date
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration[$"{nameof(AuthenticationOptions)}:Jwt:Issuer"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration[$"{nameof(AuthenticationOptions)}:Jwt:Key"]))
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrWhiteSpace(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });
    }
}
