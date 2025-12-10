using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.ExpenseType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IExpenseTypeService
    {
        Task<PagedResult<ExpenseTypeDto>> GetAllAsync(int? pageNumber = null, int? pageSize = null);
        Task<PagedResult<ExpenseTypeDto>> GetAllByUserIdAsync(int userId, int? pageNumber = null, int? pageSize = null);
        Task<ExpenseTypeDto?> GetByIdAsync(int id);
        Task<ExpenseTypeDto> CreateAsync(CreateExpenseTypeRequestDto dto, int userId);
        Task<ExpenseTypeDto?> UpdateAsync(int id, UpdateExpenseTypeRequestDto dto);
        Task<bool> DeleteAsync(int id); // soft delete (IsActive = false)
        Task<List<ExpenseTypeDto>> GetActivesAsync();
        Task<List<ExpenseTypeDto>> GetActivesByUserIdAsync(int userId);
    }
}
