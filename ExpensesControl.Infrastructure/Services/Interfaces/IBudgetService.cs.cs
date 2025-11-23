using ExpensesControl.Application.Dtos.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IBudgetService
    {
        Task<List<BudgetDto>> GetByMonthAsync(int userId, int year, int month);
        Task<BudgetDto> CreateAsync(int userId, CreateBudgetRequestDto dto);
        Task<BudgetDto?> UpdateAsync(int id, int userId, UpdateBudgetRequestDto dto);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
