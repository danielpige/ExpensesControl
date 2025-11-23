using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Report
{
    public class BudgetVsExecutionDto
    {
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; } = null!;
        public decimal TotalBudget { get; set; }
        public decimal TotalExecuted { get; set; }
    }
}
