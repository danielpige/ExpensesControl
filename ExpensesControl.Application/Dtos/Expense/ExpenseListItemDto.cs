using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class ExpenseListItemDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MoneyFundId { get; set; }
        public string MoneyFundName { get; set; } = null!;
        public string? MerchantName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
