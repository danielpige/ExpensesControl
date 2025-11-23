using ExpensesControl.Application.Dtos.Expense;
using ExpensesControl.Application.Dtos.ExpenseType;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Infrastructure.Persistence;
using ExpensesControl.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateExpenseResponseDto> CreateExpenseAsync(int userId, CreateExpenseRequestDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var fund = await _context.MoneyFunds
                    .FirstOrDefaultAsync(f => f.Id == dto.MoneyFundId);

                if (fund == null)
                    throw new InvalidOperationException("Money fund not found.");

                var total = dto.Details.Sum(d => d.Amount);

                // OPCIONAL: Validar que tenga saldo suficiente (solo si quieres)
                // if (fund.CurrentBalance < total)
                //     throw new InvalidOperationException("Insufficient balance in money fund.");

                var header = new ExpenseHeader
                {
                    Date = dto.Date,
                    UserId = userId,
                    MoneyFundId = dto.MoneyFundId,
                    Comments = dto.Comments,
                    MerchantName = dto.MerchantName,
                    DocumentType = dto.DocumentType,
                    TotalAmount = total
                };

                await _context.ExpenseHeaders.AddAsync(header);
                await _context.SaveChangesAsync();

                foreach (var d in dto.Details)
                {
                    var detail = new ExpenseDetail
                    {
                        ExpenseHeaderId = header.Id,
                        ExpenseTypeId = d.ExpenseTypeId,
                        Amount = d.Amount,
                        Comments = d.Comments
                    };

                    await _context.ExpenseDetails.AddAsync(detail);
                }

                fund.CurrentBalance -= total;
                _context.MoneyFunds.Update(fund);

                await _context.SaveChangesAsync();

                var overruns = await CalculateBudgetOverrunsAsync(userId, header.Date, dto);

                await tx.CommitAsync();

                return new CreateExpenseResponseDto
                {
                    ExpenseId = header.Id,
                    Overruns = overruns
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<List<BudgetOverrunDto>> CalculateBudgetOverrunsAsync(int userId, DateTime date, CreateExpenseRequestDto dto)
        {
            var result = new List<BudgetOverrunDto>();

            var year = date.Year;
            var month = date.Month;
            var expenseTypeIds = dto.Details.Select(d => d.ExpenseTypeId).Distinct().ToList();

            var budgets = await _context.Budgets
                .Include(b => b.ExpenseType)
                .Where(b => b.UserId == userId
                         && b.Year == year
                         && b.Month == month
                         && expenseTypeIds.Contains(b.ExpenseTypeId))
                .ToListAsync();

            var execution = await _context.ExpenseHeaders
                .Where(h => h.UserId == userId && h.Date.Year == year && h.Date.Month == month)
                .Join(_context.ExpenseDetails,
                    h => h.Id,
                    d => d.ExpenseHeaderId,
                    (h, d) => new { h, d })
                .Where(x => expenseTypeIds.Contains(x.d.ExpenseTypeId))
                .GroupBy(x => x.d.ExpenseTypeId)
                .Select(g => new { ExpenseTypeId = g.Key, Amount = g.Sum(x => x.d.Amount) })
                .ToListAsync();

            foreach (var exec in execution)
            {
                var budget = budgets.FirstOrDefault(b => b.ExpenseTypeId == exec.ExpenseTypeId);
                if (budget == null) continue;

                if (exec.Amount > budget.Amount)
                {
                    var overrunAmount = exec.Amount - budget.Amount;

                    result.Add(new BudgetOverrunDto
                    {
                        ExpenseTypeId = exec.ExpenseTypeId,
                        ExpenseTypeName = budget.ExpenseType.Name,
                        BudgetAmount = budget.Amount,
                        ExecutedAmount = exec.Amount,
                        OverrunAmount = overrunAmount
                    });
                }
            }

            return result;
        }

        public async Task<ExpenseDto?> GetByIdAsync(int userId, int expenseId)
        {
            var expense = await _context.ExpenseHeaders
                .Include(h => h.MoneyFund)
                .Include(h => h.Details)
                    .ThenInclude(d => d.ExpenseType)
                .Where(h => h.Id == expenseId && h.UserId == userId)
                .Select(h => new ExpenseDto
                {
                    Id = h.Id,
                    Date = h.Date,
                    MoneyFundId = h.MoneyFundId,
                    MoneyFundName = h.MoneyFund.Name,
                    Comments = h.Comments,
                    MerchantName = h.MerchantName,
                    DocumentType = h.DocumentType.ToString(),
                    TotalAmount = h.TotalAmount,
                    Details = h.Details
                        .Select(d => new ExpenseDetailDto
                        {
                            Id = d.Id,
                            ExpenseTypeId = d.ExpenseTypeId,
                            ExpenseTypeName = d.ExpenseType.Name,
                            Amount = d.Amount,
                            Comments = d.Comments
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return expense;
        }

        public async Task<List<ExpenseListItemDto>> GetByDateRangeAsync(int userId, DateTime from, DateTime to, int? moneyFundId)
        {
            var query = _context.ExpenseHeaders
                .Include(h => h.MoneyFund)
                .Where(h =>
                    h.UserId == userId &&
                    h.Date >= from &&
                    h.Date <= to);

            if (moneyFundId.HasValue)
            {
                query = query.Where(h => h.MoneyFundId == moneyFundId.Value);
            }

            return await query
                .OrderByDescending(h => h.Date)
                .Select(h => new ExpenseListItemDto
                {
                    Id = h.Id,
                    Date = h.Date,
                    MoneyFundId = h.MoneyFundId,
                    MoneyFundName = h.MoneyFund.Name,
                    MerchantName = h.MerchantName,
                    TotalAmount = h.TotalAmount
                })
                .ToListAsync();
        }

    }

}
