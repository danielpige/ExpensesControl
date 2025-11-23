using ExpensesControl.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Expense
{
    public class CreateExpenseRequestDto
    {
        public DateTime Date { get; set; }
        public int MoneyFundId { get; set; }

        public string? Comments { get; set; }
        public string? MerchantName { get; set; }
        public DocumentType DocumentType { get; set; }

        public List<CreateExpenseDetailDto> Details { get; set; } = new();
    }
}
