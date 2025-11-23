using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<ExpenseHeader> ExpenseHeaders { get; set; } = new List<ExpenseHeader>();
        public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
    }
}
