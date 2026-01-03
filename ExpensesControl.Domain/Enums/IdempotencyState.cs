using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Enums
{
    public enum IdempotencyState
    {
        InProgress = 1,
        Completed = 2,
    }
}
