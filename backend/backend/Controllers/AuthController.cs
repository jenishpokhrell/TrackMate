using backend.Core.Interfaces.IServices;
using backend.Dto.Auth;
using backend.Model.Dto.Auth;
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
        public async Task<IActionResult> RegisterIndividual([FromBody] RegisterUser userDto)
        {
            try
            {
                var individual = await _authService.RegisterIndividualAsync(userDto);
                return StatusCode(individual.StatusCode, individual.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register/duo/person1")]
        public async Task<IActionResult> RegisterDuoPerson1([FromBody] RegisterUser userDto)
        {
            try
            {
                var person1 = await _authService.RegisterDuoPerson1Async(userDto);
                return StatusCode(person1.StatusCode, person1.Message);
            }
            catch(ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("register/duo/person2")]
        public async Task<IActionResult> RegisterDuoPerson2([FromBody] RegisterUser userDto)
        {
            try
            {
                var person2 = await _authService.RegisterDuoPerson2Async(userDto);
                return StatusCode(person2.StatusCode, person2.Message);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _authService.LoginAsync(loginDto);
                if(user is null)
                {
                    return Unauthorized("Invalid Credentials");
                }

                return Ok(user);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
