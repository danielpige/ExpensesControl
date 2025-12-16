using ExpensesControl.Application.Dtos.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<BudgetVsExecutionDto>> GetBudgetVsExecutionAsync(
            int userId,
            DateTime from,
            DateTime to);
    }
}
