using backend.Core.Interfaces.IServices;
using backend.Dto.Expense;
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
            try
            {
                var result = await _expenseService.AddExpensesAsync(User, addExpenseDto);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("total-expenses")]
        [Authorize]
        public async Task<IActionResult> GetTotalExpenses()
        {
            try
            {
                var result = await _expenseService.GetMyTotalExpensesAsync(User);
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
