using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class Budget : BaseEntity
    {
        public int UserId { get; set; }
        public int ExpenseTypeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Amount { get; set; }

        public User User { get; set; } = null!;
        public ExpenseType ExpenseType { get; set; } = null!;
    }
}
