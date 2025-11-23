using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class ExpenseDetail : BaseEntity
    {
        public int ExpenseHeaderId { get; set; }
        public int ExpenseTypeId { get; set; }
        public decimal Amount { get; set; }
        public string? Comments { get; set; }

        public ExpenseHeader ExpenseHeader { get; set; } = null!;
        public ExpenseType ExpenseType { get; set; } = null!;
    }
}
