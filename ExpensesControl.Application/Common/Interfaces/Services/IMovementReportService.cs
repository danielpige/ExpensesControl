using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Interfaces.Services
{
    public interface IMovementReportService
    {
        Task<byte[]> ExportMovementsToExcelAsync(int userId, DateTime from, DateTime to, int? moneyFundId);
    }
}
