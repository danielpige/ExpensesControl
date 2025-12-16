using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.ExpenseType;
using ExpensesControl.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesControl.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseTypesController : ControllerBase
    {
        private readonly IExpenseTypeService _expenseTypeService;

        public ExpenseTypesController(IExpenseTypeService expenseTypeService)
        {
            _expenseTypeService = expenseTypeService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await _expenseTypeService.GetAllAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ExpenseTypeDto>>.Ok(data));
        }

        [HttpGet("get-all-by-current-user")]
        public async Task<IActionResult> GetAllByCurrentUser([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await _expenseTypeService.GetAllByUserIdAsync(GetUserId(), pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<ExpenseTypeDto>>.Ok(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _expenseTypeService.GetByIdAsync(id);
            if (item == null)
                return NotFound(ApiErrorResponse.Fail("Expense type not found."));

            return Ok(ApiResponse<ExpenseTypeDto>.Ok(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseTypeRequestDto dto)
        {
            try
            {
                var created = await _expenseTypeService.CreateAsync(dto, GetUserId());
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<ExpenseTypeDto>.Ok(created, "Expense type created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiErrorResponse.Fail(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseTypeRequestDto dto)
        {
            var updated = await _expenseTypeService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(ApiErrorResponse.Fail("Expense type not found."));

            return Ok(ApiResponse<ExpenseTypeDto>.Ok(updated, "Expense type updated successfully."));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _expenseTypeService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiErrorResponse.Fail("Expense type not found."));

            return Ok(ApiResponse<string>.Ok("Expense type disabled successfully."));
        }

        [HttpGet("actives")]
        public async Task<ActionResult<ApiResponse<List<ExpenseTypeDto>>>> GetActiveS()
        {
            var items = await _expenseTypeService.GetActivesAsync();
            return Ok(ApiResponse<List<ExpenseTypeDto>>.Ok(items));
        }

        [HttpGet("get-all-by-current-user/actives")]
        public async Task<ActionResult<ApiResponse<List<ExpenseTypeDto>>>> GetAllActivesByCurrentUser()
        {
            var items = await _expenseTypeService.GetActivesByUserIdAsync(GetUserId());
            return Ok(ApiResponse<List<ExpenseTypeDto>>.Ok(items));
        }

    }
}
