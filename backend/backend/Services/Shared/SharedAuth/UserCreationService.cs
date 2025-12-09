using backend.Dto.Auth;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class UserCreationService : IUserCreationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserCreationService> _logger;

        public UserCreationService(UserManager<ApplicationUser> userManager, ILogger<UserCreationService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<ApplicationUser> CreateUserAsync(RegisterUser userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.Username,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning($"User creation failed for {userDto.Username}. Errors: {errors}");
                throw new AuthException($"User Creation failed: {errors}");
            }

            _logger.LogInformation($"User creation for {userDto.Username} succeded.");

            return user;
        }
    }
}
