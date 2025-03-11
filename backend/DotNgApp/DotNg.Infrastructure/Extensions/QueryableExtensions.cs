namespace DotNg.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Pagination<T>(this IQueryable<T> query, int? pageNumber = null, int? pageSize = null) where T : class
    {
        pageNumber ??= 1;
        pageSize ??= 20;

        return query.Skip(((int)pageNumber - 1) * (int)pageSize).Take((int)pageSize);
    }
}