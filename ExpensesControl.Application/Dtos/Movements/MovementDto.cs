using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Movements
{
    public class MovementDto
    {
        public DateTime Date { get; set; }
        public string MovementType { get; set; } = null!; // "Expense" | "Deposit"
        public int ReferenceId { get; set; }              // ExpenseHeader.Id o Deposit.Id
        public int MoneyFundId { get; set; }
        public string MoneyFundName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }          // MerchantName o "Deposit"
    }
}
