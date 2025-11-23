using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Deposit
{
    public class CreateDepositRequestDto
    {
        public DateTime Date { get; set; }
        public int MoneyFundId { get; set; }
        public decimal Amount { get; set; }
    }
}
