using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class CreateExpenseDetailDto
    {
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? Comments { get; set; }
    }
}
