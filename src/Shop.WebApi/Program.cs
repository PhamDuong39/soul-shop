// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Com.Ctrip.Framework.Apollo.Logging;
using Serilog;
using Serilog.Debugging;
using Shop.Module.Core.Extensions;
using Shop.WebApi.Extensions;

namespace Shop.WebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var webHost = CreateHostBuilder(args).Build();

        //// AspNetCoreRateLimit
        //using (var scope = webHost.Services.CreateScope())
        //{
        //    // get the ClientPolicyStore instance
        //    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();

        //    // seed Client data from appsettings
        //    await clientPolicyStore.SeedAsync();

        //    // get the IpPolicyStore instance
        //    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

        //    // seed IP data from appsettings
        //    await ipPolicyStore.SeedAsync();
        //}
        //use startup.cs


        await webHost.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
            .ConfigureAppConfiguration((builderContext, config) =>
            {
                var env = builderContext.HostingEnvironment;

                var configuration = config
                    .AddJsonFile($"appsettings.json", true, true)
                    .AddJsonFile($"appsettings.Modules.json", true, true)
                    .AddJsonFile($"appsettings.RateLimiting.json", true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                    .Build();

                // Check if Apollo configuration is enabled
                var apolloEnabled = configuration.GetSection("ApolloEnabled").Get<bool>();
                if (apolloEnabled == true)
                {
                    var apolloConfigurationBuilder = config.AddApollo(configuration.GetSection("Apollo"));

                    // If in development environment, prioritize local configurations
                    if (env.IsDevelopment())
                    {
                        LogManager.UseConsoleLogging(Com.Ctrip.Framework.Apollo.Logging.LogLevel.Trace);

                        apolloConfigurationBuilder
                            .AddJsonFile($"appsettings.json", true)
                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
                    }

                    configuration = apolloConfigurationBuilder.Build();
                }

                // Configure Entity Framework with custom data store using Apollo configuration
                config.AddEntityFrameworkConfig(opt => opt.UseCustomizedDataStore(configuration));

                // Configure Serilog for logging
                var loggerConfig = new LoggerConfiguration();
                if (env.IsDevelopment())
                {
                    loggerConfig.MinimumLevel.Information().Enrich.FromLogContext().WriteTo.Console();
                    SelfLog.Enable(Console.Error);
                }

                Log.Logger = loggerConfig.ReadFrom.Configuration(configuration).CreateLogger();
            });
    }
}
