using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class ExpenseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MoneyFundId { get; set; }
        public string MoneyFundName { get; set; } = null!;
        public string? Comments { get; set; }
        public string? MerchantName { get; set; }
        public string DocumentType { get; set; } = null!; // enum ToString()
        public decimal TotalAmount { get; set; }

        public List<ExpenseDetailDto> Details { get; set; } = new();
    }
}
