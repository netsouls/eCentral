using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace eCentral.Core
{
    /// <summary>
    /// All querable extension
    /// </summary>
    public static class QueryableExtensions
    {
        public static object ElementAt ( this IQueryable source, int index)
        {
            Guard.IsNotNull(source, "source");
            Guard.IsNotZeroOrNegative(index, "index");

            return source.Provider.Execute(Expression.Call(typeof(Queryable), "ElementAt", new Type[] { source.ElementType },
                new Expression[] { source.Expression, Expression.Constant(index) }));
        }

        public static IQueryable OrderBy(this IQueryable source, LambdaExpression keySelector)
        {
            return source.CallQueryableMethod("OrderBy", keySelector);
        }
        
        public static IQueryable OrderByDescending(this IQueryable source, LambdaExpression keySelector)
        {
            return source.CallQueryableMethod("OrderByDescending", keySelector);
        }

        public static IQueryable OrderBy(this IQueryable source, LambdaExpression keySelector, ListSortDirection? sortDirection)
        {
            if (!sortDirection.HasValue)
            {
                return source;
            }

            if (sortDirection.Value == ListSortDirection.Ascending)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderByDescending(keySelector);
        }

        public static IQueryable SortBy(this IQueryable source, string propertyName, ListSortDirection sortOrder)
        {
            Guard.IsNotNull(source, "source");
            Guard.IsNotNullOrEmpty(propertyName, "propertyName");

            ParameterExpression parameterExpression = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
            LambdaExpression lambadaExpression = Expression.Lambda(memberExpression, parameterExpression);

            return source.OrderBy(lambadaExpression, sortOrder);
        }

        /// <summary>
        /// Sort the query results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="propertyName"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName, ListSortDirection sortOrder)
        {
            Guard.IsNotNull(source, "source");
            Guard.IsNotNullOrEmpty(propertyName, "propertyName");

            ParameterExpression parameterExpression = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression memberExpression = Expression.Property(parameterExpression, propertyName);
            LambdaExpression lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);

            string methodName = (sortOrder == ListSortDirection.Descending) ? "OrderByDescending" : "OrderBy";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, memberExpression.Type },
                                                source.Expression, Expression.Quote(lambdaExpression));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }
        
        public static IQueryable Skip(this IQueryable source, int count)
        {
            Guard.IsNotNull(source, "source");

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Skip", new Type[] { source.ElementType }, 
                new Expression[] { source.Expression, Expression.Constant(count) }));
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            Guard.IsNotNull(source, "source");

            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Take", new Type[] { source.ElementType }, 
                new Expression[] { source.Expression, Expression.Constant(count) }));
        }

        public static IQueryable Select(this IQueryable source, LambdaExpression selector)
        {
            return source.CallQueryableMethod("Select", selector);
        }

        public static IQueryable Where(this IQueryable source, Expression predicate)
        {
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType }, 
                new Expression[] { source.Expression, Expression.Quote(predicate) }));
        }

        public static IQueryable Page(this IQueryable source, int pageIndex, int pageSize)
        {
            IQueryable queryable = source;

            if (pageIndex > 0)
                queryable = queryable.Skip(pageIndex * pageSize);

            if (pageSize > 0)
                queryable = queryable.Take(pageSize);

            return queryable;
        }

        public static IList ToIList(this IQueryable source)
        {
            Guard.IsNotNull(source, "source");

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { source.ElementType }));
            foreach (object value in source)
            {
                list.Add(value);
            }

            return list;
        }

        public static IEnumerable<TItem> GetAncestors<TItem>(this TItem item, Func<TItem, TItem> getParentFunc)
        {
            if (getParentFunc == null)
            {
                throw new ArgumentNullException("getParentFunc");
            }
            if (ReferenceEquals(item, null)) yield break;
            for (TItem curItem = getParentFunc(item); !ReferenceEquals(curItem, null); curItem = getParentFunc(curItem))
            {
                yield return curItem;
            }
        }

        #region Utilities

        /// <summary>
        /// Runs the complied query
        /// </summary>
        /// <param name="source"></param>
        /// <param name="methodName"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        private static IQueryable CallQueryableMethod(this IQueryable source, string methodName, LambdaExpression selector)
        {
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), methodName, new Type[] { source.ElementType, selector.Body.Type },
                new Expression[] { source.Expression, Expression.Quote(selector) }));
        }

        #endregion
    }
}
