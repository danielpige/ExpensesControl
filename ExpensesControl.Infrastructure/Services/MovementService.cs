using ExpensesControl.Application.Dtos.Movements;
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
    public class MovementService : IMovementService
    {
        private readonly AppDbContext _context;

        public MovementService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovementDto>> GetMovementsAsync(int userId, DateTime from, DateTime to, int? moneyFundId)
        {
            // GASTOS
            var expensesQuery = _context.ExpenseHeaders
                .Include(e => e.MoneyFund)
                .Where(e =>
                    e.UserId == userId &&
                    e.Date >= from &&
                    e.Date <= to);

            if (moneyFundId.HasValue)
            {
                expensesQuery = expensesQuery.Where(e => e.MoneyFundId == moneyFundId.Value);
            }

            var expenses = await expensesQuery
                .Select(e => new MovementDto
                {
                    Date = e.Date,
                    MovementType = "Gasto",
                    ReferenceId = e.Id,
                    MoneyFundId = e.MoneyFundId,
                    MoneyFundName = e.MoneyFund.Name,
                    Amount = e.TotalAmount,           // egreso total de la factura
                    Description = e.MerchantName      // o e.Comments
                })
                .ToListAsync();

            // DEPÓSITOS
            var depositsQuery = _context.Deposits
                .Include(d => d.MoneyFund)
                .Where(d =>
                    d.UserId == userId &&
                    d.Date >= from &&
                    d.Date <= to);

            if (moneyFundId.HasValue)
            {
                depositsQuery = depositsQuery.Where(d => d.MoneyFundId == moneyFundId.Value);
            }

            var deposits = await depositsQuery
                .Select(d => new MovementDto
                {
                    Date = d.Date,
                    MovementType = "Deposito",
                    ReferenceId = d.Id,
                    MoneyFundId = d.MoneyFundId,
                    MoneyFundName = d.MoneyFund.Name,
                    Amount = d.Amount,                // ingreso
                    Description = "Deposito"
                })
                .ToListAsync();

            // Unificar y ordenar
            var result = expenses
                .Concat(deposits)
                .OrderByDescending(m => m.Date)
                .ThenBy(m => m.MovementType)
                .ToList();

            return result;
        }
    }
}
