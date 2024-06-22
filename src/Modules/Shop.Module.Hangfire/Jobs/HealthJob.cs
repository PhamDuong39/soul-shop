using Hangfire;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shop.Module.Hangfire.Jobs;

public class HealthJob(ILogger<HealthJob> logger) : BackgroundService
{
    private readonly ILogger _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(5000, stoppingToken);

            _logger.LogInformation("Health Job Starting");

            // RecurringJob.AddOrUpdate("HealthJob",
            //     () => Console.WriteLine($"Health Job Running {DateTime.Now:yyyy:MM:dd HH:mm:ss.fff}"), Cron.Minutely());

            _logger.LogInformation("Health Job Started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health Job Error");
        }

        await Task.CompletedTask;
    }
}
