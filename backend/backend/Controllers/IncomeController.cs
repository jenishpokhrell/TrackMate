using backend.Core.Interfaces.IServices;
using backend.Dto.Income;
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
            try
            {
                var result = await _incomeService.AddIncomeAsync(addIncomeDto);
                return StatusCode(result.StatusCode, result.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
