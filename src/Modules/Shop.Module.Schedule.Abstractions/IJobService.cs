using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Shop.Module.Schedule.Abstractions;

public interface IJobService
{
    Task<string> Enqueue(Expression<Func<Task>> methodCall);


    Task<string> Schedule(Expression<Func<Task>> methodCall, TimeSpan timeSpan);


    Task AddOrUpdate(string recurringJobId, Expression<Func<Task>> methodCall, Func<string> cronExpression);
}
