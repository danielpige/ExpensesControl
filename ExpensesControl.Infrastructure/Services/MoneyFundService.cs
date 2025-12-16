using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.MoneyFund;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Infrastructure.Persistence;
using ExpensesControl.Infrastructure.Persistence.Extensions;
using ExpensesControl.Application.Common.Interfaces.Services;
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

        public async Task<PagedResult<MoneyFundDto>> GetAllAsync(int? pageNumber = 0, int? pageSize = 0)
        {

            var query = _context.MoneyFunds
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt);

            var result = await query.ToPagedResultAsync(
            pageNumber,
            pageSize,
            x => new MoneyFundDto
            {
                Id = x.Id,
                Name = x.Name,
                AccountType = x.AccountType,
                CurrentBalance = x.CurrentBalance,
                IsActive = x.IsActive,
                UserId = x.UserId,
            });

            return result;
        }

        public async Task<PagedResult<MoneyFundDto>> GetAllByUserIdAsync(int userId, int? pageNumber = 0, int? pageSize = 0)
        {

            var query = _context.MoneyFunds
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.CreatedAt);

            var result = await query.ToPagedResultAsync(
            pageNumber,
            pageSize,
            x => new MoneyFundDto
            {
                Id = x.Id,
                Name = x.Name,
                AccountType = x.AccountType,
                CurrentBalance = x.CurrentBalance,
                IsActive = x.IsActive,
                UserId = x.UserId
            });

            return result;
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

        public async Task<MoneyFundDto> CreateAsync(CreateMoneyFundRequestDto dto, int userId)
        {
            var exists = await _context.MoneyFunds
                .AnyAsync(t => t.Name.ToLower() == dto.Name.ToLower() && t.UserId == userId);

            if (exists)
                throw new InvalidOperationException("An money fund with the same name already exists.");

            var fund = new MoneyFund
            {
                Name = dto.Name,
                AccountType = dto.AccountType,
                CurrentBalance = dto.InitialBalance,
                IsActive = true,
                UserId = userId
            };

            await _context.MoneyFunds.AddAsync(fund);
            await _context.SaveChangesAsync();

            return new MoneyFundDto
            {
                Id = fund.Id,
                Name = fund.Name,
                AccountType = fund.AccountType,
                CurrentBalance = fund.CurrentBalance,
                IsActive = fund.IsActive,
                UserId = fund.UserId
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
                IsActive = fund.IsActive,
                UserId = fund.UserId
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

        public async Task<List<MoneyFundDto>> GetActivesAsync()
        {
            return await _context.MoneyFunds
                .Where(mf => mf.IsActive)
                .OrderBy(mf => mf.CreatedAt)
                .Select(mf => new MoneyFundDto
                {
                    Id = mf.Id,
                    Name = mf.Name,
                    AccountType = mf.AccountType,
                    CurrentBalance = mf.CurrentBalance,
                    IsActive = mf.IsActive,
                    UserId = mf.UserId
                })
                .ToListAsync();
        }

        public async Task<List<MoneyFundDto>> GetActivesByUserIdAsync(int userId)
        {
            return await _context.MoneyFunds
                .Where(mf => mf.IsActive && mf.UserId == userId)
                .OrderBy(mf => mf.CreatedAt)
                .Select(mf => new MoneyFundDto
                {
                    Id = mf.Id,
                    Name = mf.Name,
                    AccountType = mf.AccountType,
                    CurrentBalance = mf.CurrentBalance,
                    IsActive = mf.IsActive,
                    UserId = mf.UserId
                })
                .ToListAsync();
        }
    }
}
