using ExpensesControl.Application.Dtos.Movements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Services.Interfaces
{
    public interface IMovementService
    {
        Task<List<MovementDto>> GetMovementsAsync(int userId, DateTime from, DateTime to, int? moneyFundId);
    }
}
