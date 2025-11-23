using ExpensesControl.Application.Dtos.MoneyFund;
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
    public class MoneyFundService : IMoneyFundService
    {
        private readonly AppDbContext _context;

        public MoneyFundService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MoneyFundDto>> GetAllAsync()
        {
            return await _context.MoneyFunds
                .OrderBy(x => x.Name)
                .Select(x => new MoneyFundDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    AccountType = x.AccountType,
                    CurrentBalance = x.CurrentBalance,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<MoneyFundDto?> GetByIdAsync(int id)
        {
            return await _context.MoneyFunds
                .Where(x => x.Id == id)
                .Select(x => new MoneyFundDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    AccountType = x.AccountType,
                    CurrentBalance = x.CurrentBalance,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MoneyFundDto> CreateAsync(CreateMoneyFundRequestDto dto)
        {
            var fund = new MoneyFund
            {
                Name = dto.Name,
                AccountType = dto.AccountType,
                CurrentBalance = dto.InitialBalance,
                IsActive = true
            };

            await _context.MoneyFunds.AddAsync(fund);
            await _context.SaveChangesAsync();

            return new MoneyFundDto
            {
                Id = fund.Id,
                Name = fund.Name,
                AccountType = fund.AccountType,
                CurrentBalance = fund.CurrentBalance,
                IsActive = fund.IsActive
            };
        }

        public async Task<MoneyFundDto?> UpdateAsync(int id, UpdateMoneyFundRequestDto dto)
        {
            var fund = await _context.MoneyFunds.FirstOrDefaultAsync(x => x.Id == id);
            if (fund == null)
                return null;

            fund.Name = dto.Name;
            fund.AccountType = dto.AccountType;
            fund.IsActive = dto.IsActive;

            _context.MoneyFunds.Update(fund);
            await _context.SaveChangesAsync();

            return new MoneyFundDto
            {
                Id = fund.Id,
                Name = fund.Name,
                AccountType = fund.AccountType,
                CurrentBalance = fund.CurrentBalance,
                IsActive = fund.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fund = await _context.MoneyFunds.FirstOrDefaultAsync(x => x.Id == id);
            if (fund == null)
                return false;

            fund.IsActive = false; // Soft delete

            _context.MoneyFunds.Update(fund);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<MoneyFundDto>> GetActiveAsync()
        {
            return await _context.MoneyFunds
                .Where(mf => mf.IsActive)
                .OrderBy(mf => mf.Name)
                .Select(mf => new MoneyFundDto
                {
                    Id = mf.Id,
                    Name = mf.Name,
                    AccountType = mf.AccountType,
                    CurrentBalance = mf.CurrentBalance,
                    IsActive = mf.IsActive
                })
                .ToListAsync();
        }
    }
}
