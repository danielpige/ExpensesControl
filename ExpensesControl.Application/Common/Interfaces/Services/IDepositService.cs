using ExpensesControl.Application.Dtos.Deposit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Interfaces.Services
{
    public interface IDepositService
    {
        Task<DepositDto> CreateAsync(int userId, CreateDepositRequestDto dto);
        Task<List<DepositDto>> GetByDateRangeAsync(int userId, DateTime from, DateTime to, int? moneyFundId);
        Task<DepositDto?> GetByIdAsync(int id, int userId);
    }
}
