using System.Linq.Expressions;
using Hangfire;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Schedule.Abstractions;

namespace Soul.Shop.Module.Hangfire.Services;

public class JobService(ILogger<JobService> logger) : IJobService
{
    private readonly ILogger<JobService> _logger = logger;

    public Task<string> Enqueue(Expression<Func<Task>> methodCall)
    {
        return Task.FromResult(BackgroundJob.Enqueue(methodCall));
    }

    public Task<string> Schedule(Expression<Func<Task>> methodCall, TimeSpan timeSpan)
    {
        return Task.FromResult(BackgroundJob.Schedule(methodCall, timeSpan));
    }

    public Task AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression)
    {
        RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
        return Task.CompletedTask;
    }
}