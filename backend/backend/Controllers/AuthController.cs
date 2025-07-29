using backend.Core.Interfaces.IServices;
using backend.Dto.Auth;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("register/individual")]
        public async Task<IActionResult> RegisterIndividual(RegisterIndividualDto individualDto)
        {
            try
            {
                var individual = await _authService.RegisterIndividualAsync(individualDto);
                return StatusCode(individual.StatusCode, individual.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register/duo/person1")]
        public async Task<IActionResult> RegisterDuoPerson1(RegisterDuoPerson1Dto duoPerson1Dto)
        {
            try
            {
                var person1 = await _authService.RegisterDuoPerson1Async(duoPerson1Dto);
                return StatusCode(person1.StatusCode, person1.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register/duo/person2")]
        public async Task<IActionResult> RegisterDuoPerson2(RegisterDuoPerson2Dto duoPerson2Dto)
        {
            try
            {
                var person2 = await _authService.RegisterDuoPerson2Async(duoPerson2Dto);
                return StatusCode(person2.StatusCode, person2.Message);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
