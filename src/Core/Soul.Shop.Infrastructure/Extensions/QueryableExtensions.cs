using System.Linq.Expressions;

namespace Soul.Shop.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> OrderByName<T>(this IQueryable<T> source, string propertyName, bool isDescending)
    {
        if (source == null) throw new ArgumentException(nameof(source));

        if (string.IsNullOrWhiteSpace(propertyName)) throw new ArgumentException(nameof(propertyName));

        var type = typeof(T);
        var args = Expression.Parameter(type, "x");
        var propertyInfo = type.GetProperty(propertyName);
        if (propertyInfo != null)
        {
            Expression expression = Expression.Property(args, propertyInfo);
            type = propertyInfo.PropertyType;

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expression, args);

            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            var result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, [source, lambda]);
            //convert result to IQueryable<T>
            return (IQueryable<T>)result;
        }

        return source;
    }
}