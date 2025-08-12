using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Auth;
using backend.Helpers;
using backend.Model;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IRoleManagementService _roleManagementService;
        private readonly ApplicationDbContext _context;
        private readonly GenereteJWTToken _generateJWTToken;
        private readonly INotificationService _notificationService;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<AuthService> logger,
           IRoleManagementService roleManagementService, ApplicationDbContext context, INotificationService notificationService GenereteJWTToken generateJWTToken)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roleManagementService = roleManagementService ?? throw new ArgumentNullException(nameof(roleManagementService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _generateJWTToken = generateJWTToken ?? throw new ArgumentNullException(nameof(generateJWTToken));
        }

        public async Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterIndividualDto individualDto)
        {
            if(individualDto == null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                return ErrorResponse.CreateErrorResponse(400, "Invalid registration data provided");
            }

            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", individualDto.Username);

                using var transaction = await _context.Database.BeginTransactionAsync();
                {

                }
            }catch
            {

            }
            var user = new ApplicationUser { UserName = individualDto.Username, Email = individualDto.Email, PhoneNumber = individualDto.PhoneNumber };

            var result = await _userManager.CreateAsync(user, individualDto.Password);

            if (!result.Succeeded)
            {
                return new GeneralServiceResponseDto
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal Server Error"
                };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Type == "Individual");

            var group = new AccountGroup
            {
                Name = $"{individualDto.Username}'s Group",
                AccountTypeId = accountType.Id,
                AdminUserId = user.Id
            };

            var account = new Account
            {
                Name = individualDto.Name,
                Gender = individualDto.Gender,
                Address = individualDto.Address,
                AccountRole = null,
                ApplicationUser = user,
                AccountGroupId = group.Id
            };


            _context.AccountGroups.Add(group);
            _context.Accounts.Add(account);
            await _notificationService.AddWelcomeNotificationAsync();
            await _context.SaveChangesAsync();

            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Individual Account Created Successfully."
            };
        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson1Async(RegisterDuoPerson1Dto person1Dto)
        {
            var user = new ApplicationUser { UserName = person1Dto.Username, Email = person1Dto.Email, PhoneNumber = person1Dto.PhoneNumber };
            
            var result = await _userManager.CreateAsync(user, person1Dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return new GeneralServiceResponseDto
                {
                    StatusCode = 500,
                    Success = false,
                    Message = "Internal Server Error"
                };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "GroupAdmin");

            var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Type == "Duo");

            var group = new AccountGroup
            {
                Name = person1Dto.GroupName,
                AccountTypeId = accountType.Id,
                AdminUserId = user.Id
            };

            var account = new Account
            {
                Name = person1Dto.Name,
                Gender = person1Dto.Gender,
                Address = person1Dto.Address,
                AccountRole = AccountRole.Person1,
                ApplicationUser = user,
                AccountGroupId = group.Id
            };

            var notification = new Notification
            {
                UserId = user.Id,
                Type = "Welcome, Message",
                Message = $"Welcome, {person1Dto.Username}. You have successfully created your account",
                IsRead = false
            };

            _context.AccountGroups.Add(group);
            _context.Accounts.Add(account);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Duo Account for Person 1 Created Successfully."
            };
        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson2Async(RegisterDuoPerson2Dto person2Dto)
        {
            var group = await _context.AccountGroups.Include(g => g.Accounts).Include(g => g.AccountType)
                .FirstOrDefaultAsync(g => g.Name == person2Dto.GroupName && g.AccountType.Type == "Duo");

            if(group is null)
            {
                return new GeneralServiceResponseDto
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Group not found"
                };
            }

            var accountCount = await _context.Accounts.CountAsync(a => a.AccountGroupId == group.Id);

            if(accountCount >= group.AccountType.MaxAccounts)
            {
                return new GeneralServiceResponseDto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "Duo type exceeded number of users."
                };
            }

            var user = new ApplicationUser { UserName = person2Dto.Username, Email = person2Dto.Email, PhoneNumber = person2Dto.PhoneNumber };

            var result = await _userManager.CreateAsync(user, person2Dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return new GeneralServiceResponseDto
                {
                    Success = false,
                    StatusCode = 400,
                    Message = $"Error Creating Person 2: {errors}"
                };
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            await _userManager.AddToRoleAsync(user, "User");

            var account = new Account
            {
                Name = person2Dto.Name,
                Gender = person2Dto.Gender,
                Address = person2Dto.Address,
                AccountRole = AccountRole.Person2,
                ApplicationUser = user,
                AccountGroupId = group.Id
            };

            var notification = new Notification
            {
                UserId = user.Id,
                Type = "Welcome, Message",
                Message = $"Welcome, {person2Dto.Username}. You have successfully created your account",
                IsRead = false
            };

            _context.Accounts.Add(account);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Duo Account for Person 2 Created Successfully."
            };
        }

        public async Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user is null)
            {
                throw new Exception("User not found");
            }

            var userId = await _userManager.FindByIdAsync(user.Id);

            var account = await _context.Accounts.Include(a => a.AccountGroup)
                .FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (account is null)
            {
                throw new Exception("Account not found");
            }

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
            {
                throw new Exception("Incorrect Password");
            }

            var newToken = await _generateJWTToken.GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfo(user, roles, account);

            return new LoginServiceResponseDto
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        private UserInfo GenerateUserInfo(ApplicationUser User, IList<string> roles, Account account)
        {
            return new UserInfo
            {
                Email = User.Email,
                Username = User.UserName,
                Contact = User.PhoneNumber,
                Name = account.Name,
                Address = account.Address,
                Gender = account.Gender.ToString(),
                GroupName = account.AccountGroup?.Name,
                Roles = string.Join(", ", roles)
            };
        }
    }
}
