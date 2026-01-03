using ExpensesControl.Api.Attributes;
using ExpensesControl.Api.Common;
using ExpensesControl.Application.Common.Interfaces.Services;
using ExpensesControl.Application.Dtos.Deposit;
using ExpensesControl.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpensesControl.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DepositsController : ControllerBase
    {
        private readonly IDepositService _depositService;

        public DepositsController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue("userId")!);

        [RequireIdempotency]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepositRequestDto dto)
        {
            try
            {
                var result = await _depositService.CreateAsync(GetUserId(), dto);
                return Ok(ApiResponse<DepositDto>.Ok(result, "Deposit created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiErrorResponse.Fail(ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var deposit = await _depositService.GetByIdAsync(id, GetUserId());
            if (deposit == null)
                return NotFound(ApiErrorResponse.Fail("Deposit not found."));

            return Ok(ApiResponse<DepositDto>.Ok(deposit));
        }


        [HttpGet]
        public async Task<IActionResult> GetByRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to,
            [FromQuery] int? moneyFundId)
        {
            var data = await _depositService.GetByDateRangeAsync(GetUserId(), from, to, moneyFundId);
            return Ok(ApiResponse<List<DepositDto>>.Ok(data));
        }
    }
}
