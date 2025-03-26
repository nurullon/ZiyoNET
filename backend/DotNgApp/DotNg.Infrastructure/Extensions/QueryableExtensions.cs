using System.Linq.Expressions;

namespace DotNg.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Pagination<T>(this IQueryable<T> query, int? pageNumber = null, int? pageSize = null) where T : class
    {
        pageNumber ??= 1;
        pageSize ??= 20;

        return query.Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize);
    }

    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string orderByProperty, bool ascending)
    {
        var entityType = typeof(T);
        var property = entityType.GetProperty(orderByProperty);

        if (property == null)
            throw new ArgumentException($"Property '{orderByProperty}' not found on type '{entityType.Name}'");

        var parameter = Expression.Parameter(entityType, "x");
        var propertyAccess = Expression.Property(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        string methodName = ascending ? "OrderBy" : "OrderByDescending";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [entityType, property.PropertyType],
            source.Expression,
            Expression.Quote(orderByExpression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}