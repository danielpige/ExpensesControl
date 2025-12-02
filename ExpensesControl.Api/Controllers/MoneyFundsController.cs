using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.MoneyFund;
using ExpensesControl.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesControl.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoneyFundsController : ControllerBase
    {
        private readonly IMoneyFundService _moneyFundService;

        public MoneyFundsController(IMoneyFundService moneyFundService)
        {
            _moneyFundService = moneyFundService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await _moneyFundService.GetAllAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<MoneyFundDto>>.Ok(data));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fund = await _moneyFundService.GetByIdAsync(id);
            if (fund == null)
                return NotFound(ApiErrorResponse.Fail("Money fund not found."));

            return Ok(ApiResponse<MoneyFundDto>.Ok(fund));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMoneyFundRequestDto dto)
        {
            var created = await _moneyFundService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<MoneyFundDto>.Ok(created));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMoneyFundRequestDto dto)
        {
            var updated = await _moneyFundService.UpdateAsync(id, dto);
            if (updated == null)
                return NotFound(ApiErrorResponse.Fail("Money fund not found."));

            return Ok(ApiResponse<MoneyFundDto>.Ok(updated));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _moneyFundService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiErrorResponse.Fail("Money fund not found."));

            return Ok(ApiResponse<string>.Ok("Money fund disabled successfully."));
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<List<MoneyFundDto>>>> GetActive()
        {
            var items = await _moneyFundService.GetActiveAsync();
            return Ok(ApiResponse<List<MoneyFundDto>>.Ok(items));
        }
    }
}
