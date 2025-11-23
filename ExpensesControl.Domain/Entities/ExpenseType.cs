using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class ExpenseType : BaseEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<ExpenseDetail> ExpenseDetails { get; set; } = new List<ExpenseDetail>();
    }
}
