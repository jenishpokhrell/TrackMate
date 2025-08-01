using backend.Core.Interfaces.IServices;
using backend.Dto.Budget;
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
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        [HttpPost]
        [Route("add-budget")]
        [Authorize]
        public async Task<IActionResult> AddBudget(AddBudgetDto budgetDto)
        {
            try
            {
                var result = await _budgetService.AddBudgetAsync(User, budgetDto);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
