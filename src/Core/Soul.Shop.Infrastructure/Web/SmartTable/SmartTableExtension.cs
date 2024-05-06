using System.Linq.Expressions;
using Soul.Shop.Infrastructure.Extensions;

namespace Soul.Shop.Infrastructure.Web.SmartTable;

public static class SmartTableExtension
{
    public static SmartTableResult<TResult> ToSmartTableResult<TModel, TResult>(this IQueryable<TModel> query,
        SmartTableParam param, Expression<Func<TModel, TResult>> selector)
    {
        if (param.Pagination.Number <= 0) param.Pagination.Number = 10;

        var totalRecord = query.Count();

        query = !string.IsNullOrWhiteSpace(param.Sort.Predicate) ? query.OrderByName(param.Sort.Predicate, param.Sort.Reverse) : query.OrderByName("Id", true);

        var items = query
            .Skip(param.Pagination.Start)
            .Take(param.Pagination.Number)
            .Select(selector).ToList();

        return new SmartTableResult<TResult>
        {
            Items = items,
            TotalRecord = totalRecord,
            NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
        };
    }

    public static SmartTableResult<TResult> ToSmartTableResultNoProjection<TModel, TResult>(
        this IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector)
    {
        if (param.Pagination.Number <= 0) param.Pagination.Number = 10;

        var totalRecord = query.Count();

        query = !string.IsNullOrWhiteSpace(param.Sort.Predicate) ? query.OrderByName(param.Sort.Predicate, param.Sort.Reverse) : query.OrderByName("Id", true);

        var items = query
            .Skip(param.Pagination.Start)
            .Take(param.Pagination.Number)
            .ToList();


        return new SmartTableResult<TResult>
        {
            Items = items.AsQueryable().Select(selector),
            TotalRecord = totalRecord,
            NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
        };
    }
}