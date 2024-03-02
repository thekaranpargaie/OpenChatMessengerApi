using Shared.Model;
using System.Linq.Expressions;

namespace Shared.Extensions
{
    public static class IQueryableExtensions
    {
        public static IEnumerable<T> ToPagedList<T>(this IQueryable<T> query, PagingRequestModel pagingRequestModel)
        {
            if (pagingRequestModel.IsApi == true)
            {
                return query.ToList();
            }
            return query.Skip(pagingRequestModel.Page * pagingRequestModel.PerPage).Take(pagingRequestModel.PerPage).ToList();
        }
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, String propertyName, int? direction=0)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (String.IsNullOrEmpty(propertyName)) return source;

            var parameter = Expression.Parameter(typeof(T), "Entity");

            String[] childProperties = propertyName.Split('.');
            MemberExpression property = Expression.Property(parameter, childProperties[0]);
            for (int i = 1; i < childProperties.Length; i++)
            {
                property = Expression.Property(property, childProperties[i]);
            }

            LambdaExpression selector = Expression.Lambda(property, parameter);

            string methodName = (direction > 0) ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), methodName,
                                            new Type[] { source.ElementType, property.Type },
                                            source.Expression, Expression.Quote(selector));

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}
