using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.ExpenseType;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Infrastructure.Persistence;
using ExpensesControl.Infrastructure.Persistence.Extensions;
using ExpensesControl.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExpensesControl.Infrastructure.Services
{
    public class ExpenseTypeService : IExpenseTypeService
    {
        private readonly AppDbContext _context;

        public ExpenseTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ExpenseTypeDto>> GetAllAsync(int? pageNumber = 0, int? pageSize = 0)
        {
            var query = _context.ExpenseTypes
                .AsNoTracking()
                .OrderBy(x => x.CreatedAt);

            var result = await query.ToPagedResultAsync(
            pageNumber,
            pageSize,
            x => new ExpenseTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                IsActive = x.IsActive
            });

            return result;
        }

        public async Task<ExpenseTypeDto?> GetByIdAsync(int id)
        {
            return await _context.ExpenseTypes
                .Where(x => x.Id == id)
                .Select(x => new ExpenseTypeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ExpenseTypeDto> CreateAsync(CreateExpenseTypeRequestDto dto)
        {
            // Validar duplicado por nombre (opcional pero recomendable)
            var exists = await _context.ExpenseTypes
                .AnyAsync(t => t.Name == dto.Name);

            if (exists)
                throw new InvalidOperationException("An expense type with the same name already exists.");

            // Generar código automático
            var nextCode = await GenerateNextCodeAsync();

            var entity = new ExpenseType
            {
                Name = dto.Name,
                Code = nextCode,
                IsActive = true
            };

            await _context.ExpenseTypes.AddAsync(entity);
            await _context.SaveChangesAsync();

            return new ExpenseTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                IsActive = entity.IsActive
            };
        }

        public async Task<ExpenseTypeDto?> UpdateAsync(int id, UpdateExpenseTypeRequestDto dto)
        {
            var entity = await _context.ExpenseTypes.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return null;

            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;

            _context.ExpenseTypes.Update(entity);
            await _context.SaveChangesAsync();

            return new ExpenseTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                IsActive = entity.IsActive
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ExpenseTypes.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return false;

            // Soft delete
            entity.IsActive = false;
            _context.ExpenseTypes.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        
        private async Task<string> GenerateNextCodeAsync()
        {
            // Ejemplo formato: TG-001, TG-002, ...
            const string prefix = "TG-";

            var lastCode = await _context.ExpenseTypes
                .OrderByDescending(t => t.Code)
                .Select(t => t.Code)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (!string.IsNullOrWhiteSpace(lastCode) && lastCode.StartsWith(prefix))
            {
                var numericPart = lastCode.Substring(prefix.Length);
                if (int.TryParse(numericPart, out var number))
                {
                    nextNumber = number + 1;
                }
            }

            return $"{prefix}{nextNumber:D3}";
        }

        public async Task<List<ExpenseTypeDto>> GetActiveAsync()
        {
            return await _context.ExpenseTypes
                .Where(et => et.IsActive)
                .OrderBy(et => et.CreatedAt)
                .Select(et => new ExpenseTypeDto
                {
                    Id = et.Id,
                    Code = et.Code,
                    Name = et.Name,
                    IsActive = et.IsActive
                })
                .ToListAsync();
        }
    }
}
