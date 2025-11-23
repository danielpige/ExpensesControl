using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.MoneyFund
{
    public class UpdateMoneyFundRequestDto
    {
        public string Name { get; set; } = null!;
        public string? AccountType { get; set; }
        public bool IsActive { get; set; }
    }
}
