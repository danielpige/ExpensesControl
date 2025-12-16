using ExpensesControl.Api.Common;
using ExpensesControl.Application.Dtos.Budget;
using ExpensesControl.Application.Common.Interfaces.Services;
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
        public async Task<IActionResult> GetByMonth(int year, int month)
        {
            var data = await _budgetService.GetByMonthAsync(GetUserId(), year, month);
            return Ok(ApiResponse<List<BudgetDto>>.Ok(data));
        }

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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateBudgetRequestDto dto)
        {
            var result = await _budgetService.UpdateAsync(id, GetUserId(), dto);
            if (result == null)
                return NotFound(ApiErrorResponse.Fail("Budget not found."));

            return Ok(ApiResponse<BudgetDto>.Ok(result));
        }

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
