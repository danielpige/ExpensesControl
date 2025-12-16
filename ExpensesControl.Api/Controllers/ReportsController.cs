using ExpensesControl.Api.Common;
using ExpensesControl.Application.Dtos.Report;
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
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        // GET: /api/reports/budget-vs-execution?from=2025-01-01&to=2025-01-31
        [HttpGet("budget-vs-execution")]
        public async Task<IActionResult> GetBudgetVsExecution(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
            {
                return BadRequest(ApiErrorResponse.Fail("'from' date must be less than or equal to 'to' date."));
            }

            var data = await _reportService.GetBudgetVsExecutionAsync(GetUserId(), from, to);

            return Ok(ApiResponse<List<BudgetVsExecutionDto>>.Ok(
                data,
                "Budget vs execution report generated successfully."));
        }
    }
}
