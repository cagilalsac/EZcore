﻿#nullable disable

using EZcore.DAL;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;

namespace EZcore.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// pageOrder's OrderExpression value not ending with "DESC" is used for ascending order.
        /// Add "DESC" at the end of the pageOrder's OrderExpression value for descending order.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageOrder"></param>
        /// <returns>IQueryable</returns>
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, PageOrder pageOrder) where TEntity : Record, new()
        {
            if (string.IsNullOrWhiteSpace(pageOrder.OrderExpression))
                return query;
            return pageOrder.OrderExpression.EndsWith("DESC") ?
                query.OrderByDescending(entity => EF.Property<object>(entity, pageOrder.OrderExpression.Remove(pageOrder.OrderExpression.Length - 4))) :
                query.OrderBy(entity => EF.Property<object>(entity, pageOrder.OrderExpression));
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, PageOrder pageOrder)
        {
            pageOrder.TotalRecordsCount = query.Count();
            int recordsPerPageCount;
            if (int.TryParse(pageOrder.RecordsPerPageCount, out recordsPerPageCount))
                query = query.Skip((pageOrder.PageNumber - 1) * recordsPerPageCount).Take(recordsPerPageCount);
            return query;
        }
    }
}
