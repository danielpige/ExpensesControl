using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Dtos.ExpenseType
{
    public class CreateExpenseTypeRequestDto
    {
        public string Name { get; set; } = null!;
    }
}
