using CinemaApi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CinemaApi.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> SkipTake<T>(this IQueryable<T> items, int? pageNumber, int? pageSize)
        {
            int currentPageNumber = GetPageNumber(pageNumber);
            int currentPageSize = GetPageSize(pageSize, items?.Count());
            return items.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize);
        }

        public static IQueryable<T> OrderSkipTake<T, T2>(this IQueryable<T> items, string sort, Expression<Func<T, T2>> del, int? pageNumber, int? pageSize)
        {
            return items.GetOrdered(sort, del).SkipTake(pageNumber, pageSize);
        }

        private static IQueryable<T> GetOrdered<T, T2>(this IQueryable<T> items, string sort, Expression<Func<T, T2>> del)
        {
            switch (sort?.ToLower())
            {
                case Sort.Descending:
                    return items.OrderByDescending(del);
                case Sort.Ascending:
                    return items.OrderBy(del);
                default:
                    return items;
            }
        }
        private static int GetPageNumber(int? pageNumber) => pageNumber ?? 1;
        private static int GetPageSize(int? pageSize, int? totalCount) => pageSize ?? totalCount ?? 0;
    }
}
