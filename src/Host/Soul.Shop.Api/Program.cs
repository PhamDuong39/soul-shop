using AspNetCoreRateLimit;
using Serilog;
using Serilog.Debugging;
using Soul.Shop.Api.Extension;
using Soul.Shop.Module.Core.Extensions;

namespace Soul.Shop.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var webHost = CreateHostBuilder(args).Build();

        using (var scope = webHost.Services.CreateScope())
        {
            // get the ClientPolicyStore instance
            var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

            // seed Client data from appsettings
            await clientPolicyStore.SeedAsync();

            // get the IpPolicyStore instance
            var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

            // seed IP data from appsettings
            await ipPolicyStore.SeedAsync();
        }

        await webHost.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).ConfigureAppConfiguration(
                (builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;
                    var configuration = config
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddJsonFile($"appsettings.Modules.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                        .Build();

                    config.AddEntityFrameworkConfig(opt => opt.UseCustomizedDataStore(configuration));

                    // load up serilog configuraton
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();
                    SelfLog.Enable(Console.WriteLine);
                }).ConfigureLogging((loggingBuilder) => { loggingBuilder.AddSerilog(); });
    }
}
