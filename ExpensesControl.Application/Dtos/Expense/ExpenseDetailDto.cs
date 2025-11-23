using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class ExpenseDetailDto
    {
        public int Id { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Comments { get; set; }
    }
}
