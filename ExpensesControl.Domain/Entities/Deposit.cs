using ExpensesControl.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class Deposit : BaseEntity
    {
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public int MoneyFundId { get; set; }

        public decimal Amount { get; set; }

        public User User { get; set; } = null!;
        public MoneyFund MoneyFund { get; set; } = null!;
    }
}
