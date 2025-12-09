using backend.Core.Interfaces.IServices;
using backend.Dto.Expense;
using backend.Model.Dto.Expense;
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
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        [Route("add-expenses")]
        [Authorize]
        public async Task<IActionResult> AddExpenses([FromBody] AddExpenseDto addExpenseDto)
        {
            var result = await _expenseService.AddExpensesAsync(addExpenseDto);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpGet]
        [Route("total-expenses")]
        [Authorize]
        public async Task<IActionResult> GetTotalExpenses()
        {
            var result = await _expenseService.GetTotalExpensesAsync();
            return Ok(result);
        }

        [HttpGet]
        [Route("get-all-expenses")]
        [Authorize]
        public async Task<IActionResult> GetAllExpenses()
        {
            var result = await _expenseService.GetAllExpensesAsync();
            return Ok(result);
        }

        [HttpPut]
        [Route("update-expense/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateExpense(UpdateExpenseDto updateExpenseDto, Guid id)
        {
            var result = await _expenseService.UpdateExpenseAsync(updateExpenseDto, id);
            return StatusCode(result.StatusCode, result.Message);
        }

        [HttpDelete]
        [Route("delete-expense/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _expenseService.DeleteExpenseAsync(id);
            return StatusCode(result.StatusCode, result.Message);
        }
    }
}
