using ExpensesControl.Application.Dtos.Deposit;
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
    public class DepositService : IDepositService
    {
        private readonly AppDbContext _context;

        public DepositService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DepositDto> CreateAsync(int userId, CreateDepositRequestDto dto)
        {
            // Validar que el fondo monetario exista
            var fund = await _context.MoneyFunds
                .FirstOrDefaultAsync(f => f.Id == dto.MoneyFundId);

            if (fund == null)
                throw new InvalidOperationException("Money fund not found.");

            var deposit = new Deposit
            {
                Date = dto.Date,
                MoneyFundId = dto.MoneyFundId,
                UserId = userId,
                Amount = dto.Amount
            };

            await _context.Deposits.AddAsync(deposit);

            // Actualizar saldo del fondo monetario
            fund.CurrentBalance += dto.Amount;

            await _context.SaveChangesAsync();

            return new DepositDto
            {
                Id = deposit.Id,
                Date = deposit.Date,
                MoneyFundId = fund.Id,
                MoneyFundName = fund.Name,
                Amount = deposit.Amount
            };
        }

        public async Task<List<DepositDto>> GetByDateRangeAsync(int userId, DateTime from, DateTime to, int? moneyFundId)
        {
            var query = _context.Deposits
                .Include(d => d.MoneyFund)
                .Where(d =>
                    d.UserId == userId &&
                    d.Date >= from &&
                    d.Date <= to);

            if (moneyFundId.HasValue)
            {
                query = query.Where(d => d.MoneyFundId == moneyFundId.Value);
            }

            return await query
                .OrderByDescending(d => d.Date)
                .Select(d => new DepositDto
                {
                    Id = d.Id,
                    Date = d.Date,
                    MoneyFundId = d.MoneyFundId,
                    MoneyFundName = d.MoneyFund.Name,
                    Amount = d.Amount
                })
                .ToListAsync();
        }

        public async Task<DepositDto?> GetByIdAsync(int id, int userId)
        {
            return await _context.Deposits
                .Include(d => d.MoneyFund)
                .Where(d => d.Id == id && d.UserId == userId)
                .Select(d => new DepositDto
                {
                    Id = d.Id,
                    Date = d.Date,
                    MoneyFundId = d.MoneyFundId,
                    MoneyFundName = d.MoneyFund.Name,
                    Amount = d.Amount
                })
                .FirstOrDefaultAsync();
        }
    }
}
