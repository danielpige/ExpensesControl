using ExpensesControl.Domain.Common;
using ExpensesControl.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class ExpenseHeader : BaseEntity
    {
        public DateTime Date { get; set; }

        public int UserId { get; set; }
        public int MoneyFundId { get; set; }

        public string? Comments { get; set; }
        public string? MerchantName { get; set; }
        public DocumentType DocumentType { get; set; }

        public decimal TotalAmount { get; set; }

        public User User { get; set; } = null!;
        public MoneyFund MoneyFund { get; set; } = null!;
        public ICollection<ExpenseDetail> Details { get; set; } = new List<ExpenseDetail>();
    }
}
