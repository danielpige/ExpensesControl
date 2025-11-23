using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class CreateExpenseResponseDto
    {
        public int ExpenseId { get; set; }
        public List<BudgetOverrunDto> Overruns { get; set; } = new();

        public int OverrunCount => Overruns?.Count ?? 0;
        public bool HasOverruns => OverrunCount > 0;
    }
}
