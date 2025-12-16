using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Models.Pagination;
using ExpensesControl.Application.Dtos.MoneyFund;
using ExpensesControl.Application.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await _moneyFundService.GetAllAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PagedResult<MoneyFundDto>>.Ok(data));
        }

        [HttpGet("get-all-by-current-user")]
        public async Task<IActionResult> GetAllByCurrentUser([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var data = await _moneyFundService.GetAllByUserIdAsync(GetUserId(), pageNumber, pageSize);
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
            var created = await _moneyFundService.CreateAsync(dto, GetUserId());
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

        [HttpGet("actives")]
        public async Task<ActionResult<ApiResponse<List<MoneyFundDto>>>> GetActives()
        {
            var items = await _moneyFundService.GetActivesAsync();
            return Ok(ApiResponse<List<MoneyFundDto>>.Ok(items));
        }

        [HttpGet("get-all-by-current-user/actives")]
        public async Task<ActionResult<ApiResponse<List<MoneyFundDto>>>> GetActivesByCurrentUser()
        {
            var items = await _moneyFundService.GetActivesByUserIdAsync(GetUserId());
            return Ok(ApiResponse<List<MoneyFundDto>>.Ok(items));
        }
    }
}
