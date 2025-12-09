using backend.Core.Interfaces.IServices;
using backend.Dto.Income;
using backend.Model.Dto.Income;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly IIncomeService _incomeService;

        public IncomeController(IIncomeService incomeService)
        {
            _incomeService = incomeService;
        }

        [HttpPost]
        [Route("add-income")]
        [Authorize]
        public async Task<IActionResult> AddIncome([FromBody] AddIncomeDto addIncomeDto)
        {
            var result = await _incomeService.AddIncomeAsync(addIncomeDto);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpGet]
        [Route("get-incomes")]
        [Authorize]
        public async Task<IActionResult> GetIncomes()
        {
            var result = await _incomeService.GetIncomesAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("get-total-income-amount")]
        [Authorize]
        public async Task<IActionResult> GetTotalIncomeAmount()
        {
            var result = await _incomeService.GetTotalIncomeAsync();
            return Ok(result);
        }

        [HttpPut]
        [Route("update-income")]
        [Authorize]
        public async Task<IActionResult> UpdateIncome([FromBody] UpdateIncomeDto updateIncomeDto, Guid Id)
        {
            var result = await _incomeService.UpdateIncomeAsync(updateIncomeDto, Id);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpDelete]
        [Route("delete-income")]
        [Authorize]
        public async Task<IActionResult> DeleteIncome(Guid Id)
        {
            var result = await _incomeService.DeleteIncomesAsync(Id);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
