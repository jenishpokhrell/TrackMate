using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Auth;
using backend.Helpers;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Model.Dto.Shared;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly IUserCreationService _userCreationService;
        private readonly IGroupCreationService _groupCreationService;
        private readonly IAccountCreationService _accountCreationService;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<AuthService> logger,
           IRoleManagementService roleManagementService, ApplicationDbContext context, INotificationService notificationService, GenereteJWTToken generateJWTToken,
           IUserCreationService userCreationService, IGroupCreationService groupCreationService, IAccountCreationService accountCreationService)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roleManagementService = roleManagementService ?? throw new ArgumentNullException(nameof(roleManagementService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _generateJWTToken = generateJWTToken ?? throw new ArgumentNullException(nameof(generateJWTToken));
            _userCreationService = userCreationService ?? throw new ArgumentNullException(nameof(userCreationService));
            _groupCreationService = groupCreationService ?? throw new ArgumentNullException(nameof(groupCreationService));
            _accountCreationService = accountCreationService ?? throw new ArgumentNullException(nameof(accountCreationService));
        }

        public async Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterUser userDto)
        {
            if(userDto == null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                return ErrorResponse.CreateErrorResponse(400, "Invalid registration data provided");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", userDto.Username);

                {
                    var createUser = await _userCreationService.CreateUserAsync(userDto);

                    await _roleManagementService.EnsureRoleExistsAsync(createUser, StaticUserRoles.USER);

                    var group = new AccountGroupDto
                    {
                        Name = userDto.GroupName,
                        AdminUserId = createUser.Id
                    };

                    var createdGroup = await _groupCreationService.CreateIndividualGroupAsync(group);

                    var account = new AccountDto
                    {
                        Name = userDto.Name,
                        Gender = userDto.Gender,
                        Address = userDto.Address,
                        UserId = createUser.Id,
                        AccountGroupId = createdGroup.Id
                    };

                    await _accountCreationService.IndividualAccountCreationAsync(account);

                    var notification = new AddNotificationDto
                    {
                        UserId = createUser.Id,
                        Type = StaticNotificationTypes.welcome,
                        Message = $"Welcome {account.Name}, you have successfully created your account.",
                        IsRead = false
                    };

                    var dbTransaction = transaction.GetDbTransaction();
                    await _notificationService.WelcomeNotificationAsync(notification, dbTransaction);

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Registration for {Name} done successfully.", userDto.Username);

                    await transaction.CommitAsync();
                }
            }
            catch(Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync(); 
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Transaction already completed, skipping rollback.");
                }
                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new UserRegistrationException("An error occured while registering user", ex);
            }
            
            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Individual Account Created Successfully."
            };

        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson1Async(RegisterUser userDto)
        {
            if(userDto is null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                return ErrorResponse.CreateErrorResponse(400, "Invalid registration data provided");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", userDto.Username);

                var createUser = await _userCreationService.CreateUserAsync(userDto);

                await _roleManagementService.EnsureRoleExistsAsync(createUser, StaticUserRoles.USER);

                await _userManager.AddToRoleAsync(createUser, StaticUserRoles.GROUPADMIN);

                var accountGroup = new AccountGroupDto
                {
                    Name = userDto.GroupName,
                    AdminUserId = createUser.Id,
                };

                var createdDuoGroup = await _groupCreationService.CreateDuoGroupAsync(accountGroup);

                var duoAccount1 = new AccountDto
                {
                    Name = userDto.Name,
                    Gender = userDto.Gender,
                    Address = userDto.Address,
                    AccountRole = AccountRole.Person1,
                    UserId = createUser.Id,
                    AccountGroupId = createdDuoGroup.Id
                };

                await _accountCreationService.DuoPersonAccountCreationAsync(duoAccount1);

                var notification = new AddNotificationDto
                {
                    UserId = createUser.Id,
                    Type = StaticNotificationTypes.welcome,
                    Message = $"Welcome {userDto.Name}, you have successfully created your account.",
                    IsRead = false
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.WelcomeNotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Registration for {Name} done successfully.", userDto.Name);

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();  // Only if not yet completed
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Transaction already completed, skipping rollback.");
                }
                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new UserRegistrationException("An error occured while registering user", ex);
            }

            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Duo Account for Person 1 Created Successfully."
            };
        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson2Async(RegisterUser userDto)
        {
            if(userDto is null)
            {
                _logger.LogWarning("Registration attemting with null data.");
                return ErrorResponse.CreateErrorResponse(404, "Invalid registration data provided");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", userDto.Username);

                var group = await _context.AccountGroups.Include(g => g.Accounts).Include(g => g.AccountType)
                .FirstOrDefaultAsync(g => g.Name == userDto.GroupName && g.AccountType.Type == "Duo");

                if (group is null)
                {
                    return ErrorResponse.CreateErrorResponse(404, "Group Not Found");
                }

                var accountCount = await _context.Accounts.CountAsync(a => a.AccountGroupId == group.Id);

                if (accountCount >= group.AccountType.MaxAccounts)
                {
                    return ErrorResponse.CreateErrorResponse(400, "Duo type exceeded number of users. If you want to join us you can create " +
                        "your new account, Individual or Duo, whatever suits you.");
                }

                var createUser = await _userCreationService.CreateUserAsync(userDto);

                await _roleManagementService.EnsureRoleExistsAsync(createUser, StaticUserRoles.USER);

                var duoAccount2 = new AccountDto
                {
                    Name = userDto.Name,
                    Gender = userDto.Gender,
                    Address = userDto.Address,
                    AccountRole = AccountRole.Person2,
                    UserId = createUser.Id,
                    AccountGroupId = group.Id
                };

                await _accountCreationService.DuoPersonAccountCreationAsync(duoAccount2);

                var notification = new AddNotificationDto
                {
                    UserId = createUser.Id,
                    Type = StaticNotificationTypes.welcome,
                    Message = $"Welcome {userDto.Name}, you have successfully created your account.",
                    IsRead = false
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.WelcomeNotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Registration for {Name} done successfully.", userDto.Name);

                await transaction.CommitAsync();

            }
            catch(Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch(InvalidOperationException)
                {
                    _logger.LogWarning("Transaction completed. Skipping transaction rollback.");
                }

                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new UserRegistrationException("An error occured while registering user", ex);
            }

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
