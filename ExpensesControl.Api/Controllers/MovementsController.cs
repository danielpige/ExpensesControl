using ExpensesControl.Api.Common;
using ExpensesControl.Application.Dtos.Movements;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Application.Common.Interfaces.Services;
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
        private readonly IMovementReportService _movementReportService;

        public MovementsController(IMovementService movementService, IMovementReportService movementReportService)
        {
            _movementService = movementService;
            _movementReportService = movementReportService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

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

        [HttpGet("export")]
        public async Task<IActionResult> ExportMovements([FromQuery] MovementQueryRequestDto query)
        {
            var bytes = await _movementReportService.ExportMovementsToExcelAsync(GetUserId(), query.From, query.To, query.MoneyFundId);

            var fileName = $"movements_{query.From:yyyyMMdd}_{query.To:yyyyMMdd}.xlsx";

            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
