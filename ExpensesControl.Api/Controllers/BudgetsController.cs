using ExpensesControl.Api.Attributes;
using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Interfaces.Services;
using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.Budget;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesControl.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        [HttpGet]
        public async Task<IActionResult> GetByMonth([FromQuery] int year, [FromQuery] int month, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            var data = await _budgetService.GetByMonthAsync(GetUserId(), year, month, pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<BudgetDto>>.Ok(data));
        }

        [RequireIdempotency]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBudgetRequestDto dto)
        {
            try
            {
                var result = await _budgetService.CreateAsync(GetUserId(), dto);
                return Ok(ApiResponse<BudgetDto>.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiErrorResponse.Fail(ex.Message));
            }
        }

        [RequireIdempotency]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateBudgetRequestDto dto)
        {
            var result = await _budgetService.UpdateAsync(id, GetUserId(), dto);
            if (result == null)
                return NotFound(ApiErrorResponse.Fail("Budget not found."));

            return Ok(ApiResponse<BudgetDto>.Ok(result));
        }

        [RequireIdempotency]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _budgetService.DeleteAsync(id, GetUserId());
            if (!deleted)
                return NotFound(ApiErrorResponse.Fail("Budget not found."));

            return Ok(ApiResponse<string>.Ok("Budget deleted."));
        }
    }
}
