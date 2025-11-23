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
        Task<List<ExpenseTypeDto>> GetAllAsync();
        Task<ExpenseTypeDto?> GetByIdAsync(int id);
        Task<ExpenseTypeDto> CreateAsync(CreateExpenseTypeRequestDto dto);
        Task<ExpenseTypeDto?> UpdateAsync(int id, UpdateExpenseTypeRequestDto dto);
        Task<bool> DeleteAsync(int id); // soft delete (IsActive = false)
        Task<List<ExpenseTypeDto>> GetActiveAsync();
    }
}
