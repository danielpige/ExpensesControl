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
            this IQueryable<TSource> query,
            int? pageNumber,
            int? pageSize,
            Expression<Func<TSource, TDest>> selector,
            CancellationToken cancellationToken = default)
        {
            // 1. Conteo total sin paginar
            var totalCount = await query.CountAsync(cancellationToken);

            // 2. Decidir si se aplica paginación
            var usePagination = pageNumber.HasValue && pageNumber.Value > 0 &&
                                pageSize.HasValue && pageSize.Value > 0;

            if (usePagination)
            {
                var skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }
            else
            {
                pageNumber = 1;
                pageSize = totalCount == 0 ? 0 : totalCount;
            }

            // 3. Proyección a DTO + ejecución
            var items = await query
                .Select(selector)
                .ToListAsync(cancellationToken);

            // 4. Construir resultado
            return new PagedResult<TDest>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 0
            };
        }

        // OPCIONAL: overload si quieres paginar entidades sin proyectar
        public static Task<PagedResult<TSource>> ToPagedResultAsync<TSource>(
            this IQueryable<TSource> query,
            int? pageNumber,
            int? pageSize,
            CancellationToken cancellationToken = default)
        {
            return query.ToPagedResultAsync<TSource, TSource>(
                pageNumber,
                pageSize,
                x => x,
                cancellationToken);
        }
    }
}
