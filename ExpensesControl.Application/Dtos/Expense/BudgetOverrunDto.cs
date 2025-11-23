using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class BudgetOverrunDto
    {
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; } = null!;
        public decimal BudgetAmount { get; set; }
        public decimal ExecutedAmount { get; set; }
        public decimal OverrunAmount { get; set; }
    }
}
