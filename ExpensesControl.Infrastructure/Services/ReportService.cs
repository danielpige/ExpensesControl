using ExpensesControl.Application.Dtos.Report;
using ExpensesControl.Infrastructure.Persistence;
using ExpensesControl.Application.Common.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BudgetVsExecutionDto>> GetBudgetVsExecutionAsync(
            int userId,
            DateTime from,
            DateTime to)
        {
            var fromYm = from.Year * 100 + from.Month;
            var toYm = to.Year * 100 + to.Month;

            var budgetData = await _context.Budgets
                .Include(b => b.ExpenseType)
                .Where(b => b.UserId == userId)
                .Where(b => (b.Year * 100 + b.Month) >= fromYm &&
                            (b.Year * 100 + b.Month) <= toYm)
                .GroupBy(b => new { b.ExpenseTypeId, b.ExpenseType.Name })
                .Select(g => new
                {
                    g.Key.ExpenseTypeId,
                    ExpenseTypeName = g.Key.Name,
                    TotalBudget = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var executionData = await _context.ExpenseDetails
                .Include(d => d.ExpenseType)
                .Include(d => d.ExpenseHeader)
                .Where(d => d.ExpenseHeader.UserId == userId &&
                            d.ExpenseHeader.Date >= from &&
                            d.ExpenseHeader.Date <= to)
                .GroupBy(d => new { d.ExpenseTypeId, d.ExpenseType.Name })
                .Select(g => new
                {
                    g.Key.ExpenseTypeId,
                    ExpenseTypeName = g.Key.Name,
                    TotalExecuted = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var dict = new Dictionary<int, BudgetVsExecutionDto>();

            foreach (var b in budgetData)
            {
                dict[b.ExpenseTypeId] = new BudgetVsExecutionDto
                {
                    ExpenseTypeId = b.ExpenseTypeId,
                    ExpenseTypeName = b.ExpenseTypeName,
                    TotalBudget = b.TotalBudget,
                    TotalExecuted = 0m
                };
            }

            foreach (var e in executionData)
            {
                if (dict.TryGetValue(e.ExpenseTypeId, out var dto))
                {
                    dto.TotalExecuted = e.TotalExecuted;
                }
                else
                {
                    dict[e.ExpenseTypeId] = new BudgetVsExecutionDto
                    {
                        ExpenseTypeId = e.ExpenseTypeId,
                        ExpenseTypeName = e.ExpenseTypeName,
                        TotalBudget = 0m,
                        TotalExecuted = e.TotalExecuted
                    };
                }
            }

            return dict.Values
                .OrderBy(x => x.ExpenseTypeName)
                .ToList();
        }
    }
}
