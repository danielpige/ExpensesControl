using ExpensesControl.Api.Attributes;
using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Interfaces.Services;
using ExpensesControl.Application.Dtos.Expense;
using ExpensesControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        [RequireIdempotency]
        [HttpPost]
        public async Task<ActionResult> CreateExpense([FromBody] CreateExpenseRequestDto dto)
        {
            var result = await _expenseService.CreateExpenseAsync(GetUserId(), dto);

            var message = result.HasOverruns
                ? $"Budget overrun detected in {result.OverrunCount} expense type(s)."
                : "Expense created successfully.";

            return Ok(ApiResponse<CreateExpenseResponseDto>.Ok(result, message));
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _expenseService.GetByIdAsync(GetUserId(), id);
            if (expense == null)
                return NotFound(ApiErrorResponse.Fail("Expense not found."));

            return Ok(ApiResponse<ExpenseDto>.Ok(expense));
        }


        [HttpGet]
        public async Task<IActionResult> GetByRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int? moneyFundId)
        {
            if (from > to)
            {
                return BadRequest(ApiErrorResponse.Fail("'from' date must be less than or equal to 'to' date."));
            }

            var data = await _expenseService.GetByDateRangeAsync(GetUserId(), from, to, moneyFundId);
            return Ok(ApiResponse<List<ExpenseListItemDto>>.Ok(data));
        }
    }
}
