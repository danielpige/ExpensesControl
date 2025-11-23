using ExpensesControl.Application.Dtos.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<CreateExpenseResponseDto> CreateExpenseAsync(int userId, CreateExpenseRequestDto dto);

        Task<ExpenseDto?> GetByIdAsync(int userId, int expenseId);

        Task<List<ExpenseListItemDto>> GetByDateRangeAsync(int userId, DateTime from, DateTime to, int? moneyFundId);
    }
}
