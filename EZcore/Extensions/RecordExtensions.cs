#nullable disable

using EZcore.DAL;
using EZcore.Models;
using Microsoft.EntityFrameworkCore;

namespace EZcore.Extensions
{
    public static class RecordExtensions
    {
        public static TEntity Trim<TEntity>(this TEntity record) where TEntity : Record, new()
        {
            var properties = record.GetType().GetProperties().Where(property => property.PropertyType == typeof(string)).ToList();
            object value;
            if (properties is not null)
            {
                foreach (var property in properties)
                {
                    value = property.GetValue(record);
                    if (value is not null)
                        property.SetValue(record, ((string)value).Trim());
                }
            }
            return record;
        }

        /// <summary>
        /// pageOrder's OrderExpression value not ending with "DESC" is used for ascending order.
        /// Add "DESC" at the end of the pageOrder's OrderExpression value for descending order.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageOrder"></param>
        /// <returns>IOrderedQueryable</returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, PageOrder pageOrder) where TEntity : Record, new()
        {
            if (string.IsNullOrWhiteSpace(pageOrder.OrderExpression))
                return query as IOrderedQueryable<TEntity>;
            return pageOrder.OrderExpression.EndsWith("DESC") ?
                query.OrderByDescending(entity => EF.Property<object>(entity, pageOrder.OrderExpression.Remove(pageOrder.OrderExpression.Length - 4))) :
                query.OrderBy(entity => EF.Property<object>(entity, pageOrder.OrderExpression));
        }

        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> query, PageOrder pageOrder) where TEntity : Record, new()
        {
            pageOrder.TotalRecordsCount = query.Count();
            int recordsPerPageCount;
            if (int.TryParse(pageOrder.RecordsPerPageCount, out recordsPerPageCount))
                query = query.Skip((pageOrder.PageNumber - 1) * recordsPerPageCount).Take(recordsPerPageCount);
            return query;
        }
    }
}
