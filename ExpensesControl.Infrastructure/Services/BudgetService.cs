using ExpensesControl.Application.Dtos.Budget;
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
    public class BudgetService : IBudgetService
    {
        private readonly AppDbContext _context;

        public BudgetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BudgetDto>> GetByMonthAsync(int userId, int year, int month)
        {
            return await _context.Budgets
                .Include(b => b.ExpenseType)
                .Where(b =>
                    b.UserId == userId &&
                    b.Year == year &&
                    b.Month == month)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    ExpenseTypeId = b.ExpenseTypeId,
                    ExpenseTypeName = b.ExpenseType.Name,
                    Year = b.Year,
                    Month = b.Month,
                    Amount = b.Amount
                })
                .ToListAsync();
        }

        public async Task<BudgetDto> CreateAsync(int userId, CreateBudgetRequestDto dto)
        {
            // Validar duplicado (enunciado indirecto)
            var exists = await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.ExpenseTypeId == dto.ExpenseTypeId &&
                b.Year == dto.Year &&
                b.Month == dto.Month);

            if (exists)
                throw new InvalidOperationException("Budget for this type and month already exists.");

            var budget = new Budget
            {
                UserId = userId,
                ExpenseTypeId = dto.ExpenseTypeId,
                Year = dto.Year,
                Month = dto.Month,
                Amount = dto.Amount
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                ExpenseTypeId = budget.ExpenseTypeId,
                ExpenseTypeName = (await _context.ExpenseTypes.FindAsync(dto.ExpenseTypeId))!.Name,
                Year = budget.Year,
                Month = budget.Month,
                Amount = budget.Amount
            };
        }

        public async Task<BudgetDto?> UpdateAsync(int id, int userId, UpdateBudgetRequestDto dto)
        {
            var budget = await _context.Budgets
                .Include(b => b.ExpenseType)
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
                return null;

            budget.Amount = dto.Amount;
            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                ExpenseTypeId = budget.ExpenseTypeId,
                ExpenseTypeName = budget.ExpenseType.Name,
                Year = budget.Year,
                Month = budget.Month,
                Amount = budget.Amount
            };
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (budget == null)
                return false;

            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
