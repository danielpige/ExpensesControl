using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.MoneyFund
{
    public class MoneyFundDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? AccountType { get; set; }
        public decimal CurrentBalance { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
    }
}
