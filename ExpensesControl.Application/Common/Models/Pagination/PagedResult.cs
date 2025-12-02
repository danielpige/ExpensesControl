using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Models.Pagination
{
    public class PagedResult<T>
    {
        /// <summary>
        /// Registros de la página actual.
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Total de registros en la tabla/consulta (sin paginar).
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Número de página actual (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Tamaño de la página (cuántos registros por página).
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de páginas (calculado).
        /// </summary>
        public int TotalPages => PageSize == 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
