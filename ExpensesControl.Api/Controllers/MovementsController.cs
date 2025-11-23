using ExpensesControl.Api.Common;
using ExpensesControl.Application.Dtos.Movements;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesControl.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly IMovementService _movementService;

        public MovementsController(IMovementService movementService)
        {
            _movementService = movementService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        // GET: /api/movements?from=2025-01-01&to=2025-01-31&moneyFundId=1
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] MovementQueryRequestDto query)
        {
            // Si la validación falla (FluentValidation), no entra aquí.
            var data = await _movementService.GetMovementsAsync(
                GetUserId(),
                query.From,
                query.To,
                query.MoneyFundId);

            return Ok(ApiResponse<List<MovementDto>>.Ok(data));
        }
    }
}
