using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Modules;
using Shop.Module.Core.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Core.Extensions;
using Shop.WebApi.Filters;
using Shop.WebApi.Handlers;
using System.Reflection;
using System.Text;

namespace Shop.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Thêm tài liệu vênh vang
    /// </summary>
    /// <param name="services"></param>
    /// <param name="title"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
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
                    "Một hệ thống trung tâm mô-đun đơn giản, đa nền tảng được xây dựng trên .NET 8.0. <br />Hỗ trợ Swagger gọi trực tiếp giao diện mà không cần nhập token. Vui lòng sử dụng MockApi để mô phỏng hoạt động đăng nhập của người dùng, gỡ lỗi trực tuyến và gọi API. ",
                Contact = new OpenApiContact
                {
                    Name = "Circle",
                    Email = "circle@trueai.org",
                    Url = new Uri("https://github.com/trueai-org/module-shop")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://github.com/trueai-org/module-shop/blob/master/LICENSE")
                }
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

            // Nhận tất cả các cụm được tải bởi ứng dụng hiện tại
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Duyệt qua tập hợp để tìm và bao gồm các tệp chú thích XML
            foreach (var assembly in assemblies)
            {
                var assemblyName = assembly.GetName().Name;

                // Chỉ bao gồm XML cho các tập hợp có chứa từ khóa "Module"
                if (assemblyName.StartsWith("Shop.Module."))
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
            })
            .AddNewtonsoftJson(options =>
            {
                // Bỏ qua các tham chiếu vòng tròn
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Không sử dụng key vỏ lạc đà
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();

                // Đặt định dạng thời gian đầu vào/đầu ra
                options.SerializerSettings.DateTimeZoneHandling =
                    DateTimeZoneHandling.Local; // json to datetime 2019-02-26T22:34:13.000Z -> 2019-02-27 06:34:13
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        // Lỗi xác thực tự động kích hoạt phản hồi HTTP 400. Tắt tính năng tự động trả về lỗi khi ModelState không hợp lệ, chỉ được định cấu hình trong các dự án api.
        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.Configure<IdentityOptions>(options =>
        {
            // https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-2.2
            // Ký tự được phép trong tên người dùng.
            // Vì tên người dùng, email và số điện thoại di động được phép đăng nhập nên tên người dùng không được xung đột với cách đặt tên email.
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
            // Nếu được đặt thành true thì hộp thư không thể trống, do đó giá trị này không được đặt
            //options.User.RequireUniqueEmail = true;

            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/2fa?view=aspnetcore-1.1&viewFallbackFrom=aspnetcore-2.2
            // Khóa tài khoản để ngăn chặn các cuộc tấn công vũ phu
            // Default Lockout settings.
            // Số lần truy cập không thành công trước khi người dùng bị khóa, nếu khóa được bật.
            // options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            // Khoảng thời gian người dùng đã bị khóa trong khi khóa xảy ra.
            // options.Lockout.MaxFailedAccessAttempts = 5;
            // Xác định xem chức năng khóa có được bật cho người dùng mới hay không. mặc định：true
            // options.Lockout.AllowedForNewUsers = false;

            // dấu hiệu duy nhất trên
            // Kiểm soát IsNotAllowed, nếu tất cả được đặt thành true, email và điện thoại di động phải được xác minh trước khi cho phép đăng nhập. Vì vậy nó chưa được kích hoạt.
            // Default SignIn settings.
            // Yêu cầu xác nhận email, đăng nhập.
            //options.SignIn.RequireConfirmedEmail = true;
            //// Cần có số điện thoại được xác nhận để đăng nhập.
            //options.SignIn.RequireConfirmedPhoneNumber = true;
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
        services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseCustomizedDataStore(configuration);
            options.EnableDetailedErrors();
        });
    }

    public static void UseCustomizedDataStore(this DbContextOptionsBuilder options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MySqlServerVersion(new Version(8, 4, 0));

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
                    ValidateAudience = false, // Cho phép ẩn danh
                    ValidateLifetime = false, // Trong trường hợp này, chúng tôi không quan tâm đến ngày hết hạn của mã thông báo
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration[$"{nameof(AuthenticationOptions)}:Jwt:Issuer"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration[$"{nameof(AuthenticationOptions)}:Jwt:Key"]))
                };

                // Trong một số trường hợp, chúng tôi có thể sử dụng Url để chuyển Token
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
