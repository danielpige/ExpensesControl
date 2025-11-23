using ExpensesControl.Application.Dtos.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<BudgetVsExecutionDto>> GetBudgetVsExecutionAsync(
            int userId,
            DateTime from,
            DateTime to);
    }
}
