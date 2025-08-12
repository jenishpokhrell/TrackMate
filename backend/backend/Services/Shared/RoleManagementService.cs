using backend.Model;
using backend.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleManagementService> _logger;

        public RoleManagementService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<RoleManagementService> logger)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task EnsureRoleExistsAsync(ApplicationUser User, string role)
        {
            try
            {
                //Checking if the role exists
                if(!await _roleManager.RoleExistsAsync(role))
                {
                    _logger.LogInformation("Creating Role: {role}", role);

                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                           {
                        var error = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create role {role}: {errors}", role, error);
                        throw new UserRegistrationException($"Failed creating role: {error}");
                    }
                }

                // Checking if user already has a role
                if(await _userManager.IsInRoleAsync(User, role))
                {
                    _logger.LogInformation("User {UserId} already has a role {role}", User.Id, role);
                    return;
                }

                //Assigning role to user
                _logger.LogInformation("Assiging {role} role tp {UserId}", role, User.Id);
                var addRoleResult = await _userManager.AddToRoleAsync(User, role);
                if (!addRoleResult.Succeeded)
                {
                    var error = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to assign role {role} to {User.Id} : {error}");
                    throw new UserRegistrationException($"Failed assigning role: {error}");
                }

                _logger.LogInformation($"Successfully assigned {role} role to user {User.Id}");
            }
            catch (Exception ex) when (!(ex is UserRegistrationException))
            {
                _logger.LogError(ex, "Unexpected error in role management for user {UserId}", User.Id);
                throw new UserRegistrationException("An error occured during role assignment", ex);
            }
        }
    }
}
