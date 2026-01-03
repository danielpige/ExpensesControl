using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Interfaces.Services
{
    public interface IBudgetService
    {
        Task<PagedResult<BudgetDto>> GetByMonthAsync(int userId, int year, int month, int pageNumber = 1, int pageSize = 10);
        Task<BudgetDto> CreateAsync(int userId, CreateBudgetRequestDto dto);
        Task<BudgetDto?> UpdateAsync(int id, int userId, UpdateBudgetRequestDto dto);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
