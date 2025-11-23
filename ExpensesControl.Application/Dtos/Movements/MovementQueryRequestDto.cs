using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.Movements
{
    public class MovementQueryRequestDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? MoneyFundId { get; set; }
    }
}
