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
                var result = await _budgetService.AddBudgetAsync(budgetDto);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-my-budget")]
        [Authorize]
        public async Task<IActionResult> GetMyBudget()
        {
            try
            {   
                var result = await _budgetService.GetMyBudgetAsync();
                return Ok(result);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-all-budgets")]
        [Authorize]
        public async Task<IActionResult> GetAllBudgets()
        {
            try
            {
                var result = await _budgetService.GetAllBudgetsAsync();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("get-my-remaining-budget")]
        [Authorize]
        public async Task<IActionResult> GetMyRemainingBudget()
        {
            try
            {
                var result = await _budgetService.GetMyRemainingBudgetAsync();
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("update-budget/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBudget(UpdateBudgetDto updateBudgetDto, Guid id)
        {
            try
            {
                var result = await _budgetService.UpdateBudgetAsync(updateBudgetDto, id);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("delete-budget/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _budgetService.DeleteBudgetAsync(User, id);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
