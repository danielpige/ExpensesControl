using ExpensesControl.Application.Common.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Persistence.Extensions
{
    public static class QueryablePaginationExtensions
    {
        public static async Task<PagedResult<TDest>> ToPagedResultAsync<TSource, TDest>(
            this IOrderedQueryable<TSource> query,                 // obliga a OrderBy antes
            int pageNumber,
            int pageSize,
            Expression<Func<TSource, TDest>> selector,
            int maxPageSize = 200,
            bool includeTotalCount = true,
            CancellationToken cancellationToken = default)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = Math.Min(pageSize, maxPageSize);

            var totalCount = includeTotalCount
                ? await query.CountAsync(cancellationToken)
                : -1;

            var skip = (long)(pageNumber - 1) * pageSize;

            var items = await query
                .Skip((int)Math.Min(skip, int.MaxValue))            // defensa extra
                .Take(pageSize)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new PagedResult<TDest>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public static Task<PagedResult<TSource>> ToPagedResultAsync<TSource>(
            this IOrderedQueryable<TSource> query,
            int pageNumber,
            int pageSize,
            int maxPageSize = 200,
            bool includeTotalCount = true,
            CancellationToken cancellationToken = default)
            => query.ToPagedResultAsync(pageNumber, pageSize, x => x, maxPageSize, includeTotalCount, cancellationToken);
    }

}
