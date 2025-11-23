using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class MoneyFund : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? AccountType { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ExpenseHeader> Expenses { get; set; } = new List<ExpenseHeader>();
        public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
    }
}
