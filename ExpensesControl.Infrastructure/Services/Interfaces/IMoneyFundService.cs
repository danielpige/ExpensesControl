using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.MoneyFund;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IMoneyFundService
    {
        Task<PagedResult<MoneyFundDto>> GetAllAsync(int? pageNumber = 0, int? pageSize = 0);
        Task<MoneyFundDto?> GetByIdAsync(int id);
        Task<MoneyFundDto> CreateAsync(CreateMoneyFundRequestDto dto);
        Task<MoneyFundDto?> UpdateAsync(int id, UpdateMoneyFundRequestDto dto);
        Task<bool> DeleteAsync(int id); // Soft delete
        Task<List<MoneyFundDto>> GetActiveAsync();
    }
}
