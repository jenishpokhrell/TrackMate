using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Auth;
using backend.Exceptions;
using backend.Helpers;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Model.Dto.Shared;
using backend.Repositories.Interfaces;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IRoleManagementService _roleManagementService;
        private readonly ApplicationDbContext _context;
        private readonly GenereteJWTToken _generateJWTToken;
        private readonly GenerateUserInfo _userInfo;
        private readonly INotificationService _notificationService;
        private readonly IUserCreationService _userCreationService;
        private readonly IGroupCreationService _groupCreationService;
        private readonly IAccountCreationService _accountCreationService;
        private readonly IFindAccountById _findAccountById;
        private readonly IFindAccountGroupId _findAccountGroupId;
        private readonly IUserContextService _userContext;
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly IAccountGroupRepository _accountGroupRepository;
        private readonly IAccountTypeRepository _accountTypeRepository;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, IMapper mapper, ILogger<AuthService> logger,
           IRoleManagementService roleManagementService, ApplicationDbContext context, INotificationService notificationService, 
           GenereteJWTToken generateJWTToken, GenerateUserInfo userInfo, IUserCreationService userCreationService, IGroupCreationService groupCreationService,
           IAccountCreationService accountCreationService,IFindAccountById findAccountById, IFindAccountGroupId findAccountGroupId,
           IUserContextService userContext, IConfiguration configuration, IAuthRepository authRepository,
           IAccountGroupRepository accountGroupRepository, IAccountTypeRepository accountTypeRepository)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roleManagementService = roleManagementService ?? throw new ArgumentNullException(nameof(roleManagementService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _generateJWTToken = generateJWTToken ?? throw new ArgumentNullException(nameof(generateJWTToken));
            _userInfo = userInfo ?? throw new ArgumentNullException(nameof(userInfo));
            _userCreationService = userCreationService ?? throw new ArgumentNullException(nameof(userCreationService));
            _groupCreationService = groupCreationService ?? throw new ArgumentNullException(nameof(groupCreationService));
            _accountCreationService = accountCreationService ?? throw new ArgumentNullException(nameof(accountCreationService));
            _findAccountById = findAccountById ?? throw new ArgumentNullException(nameof(findAccountById));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _accountGroupRepository = accountGroupRepository ?? throw new ArgumentNullException(nameof(accountGroupRepository));
            _accountTypeRepository = accountTypeRepository ?? throw new ArgumentNullException(nameof(accountTypeRepository));
        }

        public async Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterUser userDto)
        {
            if(userDto == null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                throw new ValidationException("Registration attempt with null data");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", userDto.Username);

                {
                    var createUser = await _userCreationService.CreateUserAsync(userDto);

                    await _roleManagementService.EnsureRoleExistsAsync(createUser, StaticUserRoles.USER);
                    await _roleManagementService.EnsureRoleExistsAsync(createUser, StaticUserRoles.GROUPADMIN);

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

                    return new GeneralServiceResponseDto
                    {
                        StatusCode = 201,
                        Success = true,
                        Message = "Individual Account Created Successfully."
                    };
                }
            }
            catch(AuthException ex)
            {
                await transaction.RollbackAsync(); 
                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new AuthException("An error occured while registering user", ex);
            }
        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson1Async(RegisterUser userDto)
        {
            if(userDto is null)
            {
                _logger.LogWarning("Registration attempt with null data.");
                throw new ValidationException("Invalid registration data provided");
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

                return new GeneralServiceResponseDto
                {
                    StatusCode = 201,
                    Success = true,
                    Message = "Duo Account for Person 1 Created Successfully."
                };

            }
            catch (AuthException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new AuthException("An error occured while registering user", ex);
            }
        }

        public async Task<GeneralServiceResponseDto> RegisterDuoPerson2Async(RegisterUser userDto)
        {
            if(userDto is null)
            {
                _logger.LogWarning("Registration attemting with null data.");
                throw new ValidationException("Invalid registration data provided");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Starting user registration for username: {Username}", userDto.Username);

                var group = await _context.AccountGroups.Include(g => g.Accounts).Include(g => g.AccountType)
                .FirstOrDefaultAsync(g => g.Name == userDto.GroupName && g.AccountType.Type == "Duo");

                if (group is null)
                {
                    throw new NotFoundException("Group Not Found");
                }

                var accountCount = await _context.Accounts.CountAsync(a => a.AccountGroupId == group.Id);

                if (accountCount >= group.AccountType.MaxAccounts)
                {
                    throw new ValidationException("Duo type exceeded number of users. If you want to join us you can create " +
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
                return new GeneralServiceResponseDto
                {
                    StatusCode = 201,
                    Success = true,
                    Message = "Duo Account for Person 2 Created Successfully."
                };
            }
            catch (AuthException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Registration failed for {Name} done successfully.", userDto.Username);
                throw new AuthException("An error occured while registering user", ex);
            }      
        }

        public async Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user is null)
            {
                throw new NotFoundException("User not found");
            }

            //var userId = await _userManager.FindByIdAsync(user.Id);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
            {
                throw new UnauthorizedAccessException("User not authenticated.");
            }

            var newToken = await _generateJWTToken.GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var loginInfo = _userInfo.GenerateLoginInfo(user, roles);

            return new LoginServiceResponseDto
            {
                NewToken = newToken,
                LoginInfo = loginInfo
            };
        }

        public async Task<UserInfo> GetUserByIdAsync(string UserId)
        {
            var loggedInUserId = _userContext.GetCurrentLoggedInUserID();

            var user = await _userManager.FindByIdAsync(UserId);

            if (user is null)
                throw new NotFoundException("User not found.");

            var account = await _findAccountById.GetAccountById(UserId);
            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(loggedInUserId);
            var role = await _userManager.GetRolesAsync(user);

            if (account.AccountGroupId != accountGroupId || user.Id != loggedInUserId && role.Contains(StaticUserRoles.ADMIN))
                throw new ForbiddenException("You are not authorized to access this data.");

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = _userInfo.GenerateInfo(user, roles, account);

            return _mapper.Map<UserInfo>(userInfo);
        }

        public async Task<MeResponseDto> MeAsync(MeDto meDto)
        {
            ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meDto.Token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
            }, out SecurityToken securityToken);

            string decodedUsername = handler.Claims.First(q => q.Type == ClaimTypes.Name).Value;

            if (decodedUsername is null)
                return null;

            var user = await _userManager.FindByNameAsync(decodedUsername);
            if (user is null)
                return null;

            var account = await _findAccountById.GetAccountById(_userContext.GetCurrentLoggedInUserID());
            if (account is null)
                return null;

            var newToken = await _generateJWTToken.GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = _userInfo.GenerateInfo(user, roles, account);

            return new MeResponseDto
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        public async Task<IEnumerable<UserInfoForAdmin>> GetAllUsersAsync()
        {
            var loggedInUser = _userContext.GetCurrentLoggedInUserID();

            if (loggedInUser is null)
                throw new UnauthorizedAccessException("User not authenticated.");

            var user = await _userManager.FindByIdAsync(loggedInUser);

            if (user is null)
                throw new NotFoundException("User not found");

            var role = await _userManager.GetRolesAsync(user);
            var accounts = await _authRepository.GetAllAccounts();     

            if (!role.Contains(StaticUserRoles.ADMIN))
                throw new ForbiddenException("Only admin can access all users data");

            return _mapper.Map<IEnumerable<UserInfoForAdmin>>(accounts);
        }

        public async Task<GeneralServiceResponseDto> UpdateUserAsync(UpdateUserDto updateUserDto, string userId)
        {
            if (updateUserDto is null)
            {
                _logger.LogError("Update of data with null.");
                throw new ValidationException("Updating user with null data.");
            }

            var currentUser = _userContext.GetCurrentLoggedInUserID();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User not found");

            if (currentUser != user.Id)
                throw new ForbiddenException("You are not authorized to update this user.");

            var account = await _context.Accounts.Where(a => a.UserId == userId).FirstOrDefaultAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Username;
                user.PhoneNumber = updateUserDto.PhoneNumber;

                await _userManager.UpdateAsync(user);

                account.Name = updateUserDto.Name;
                account.Gender = updateUserDto.Gender;
                account.Address = updateUserDto.Address;

                await _authRepository.UpdateAccount(account, userId);

                var notification = new AddNotificationDto
                {
                    UserId = user.Id,
                    Type = StaticNotificationTypes.accountUpdate,
                    Message = $"{account.Name}, you have updated your account.",
                    IsRead = false
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.WelcomeNotificationAsync(notification, dbTransaction);

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "User Updated Successfully"
                };
            }
            catch(AuthException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Update failed for {Name} done successfully.", user.UserName);
                throw new AuthException("An error occured while registering user", ex);
            }
        }

        public async Task<GeneralServiceResponseDto> ChangePasswordAsync(PasswordDto passwordDto, string userId)
        {
            var currentUser = _userContext.GetCurrentLoggedInUserID();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("User not found");

            if (user.Id != currentUser)
                throw new ForbiddenException("You are not authorized to change this password.");

            var changePassword = await _userManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (!changePassword.Succeeded)
                throw new AuthException("Password didn't match");

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);
            return new GeneralServiceResponseDto()
            {
                Success = true,
                StatusCode = 200,
                Message = "Password Updated Successfully"
            };
        }

        public async Task<GeneralServiceResponseDto> DeleteUserAsync(Guid accountGroupId)
        {
            var currentUser = _userContext.GetCurrentLoggedInUserID();

            var user = await _userManager.FindByIdAsync(currentUser);

            var accountGroup = await _accountGroupRepository.GetAccountGroupById(accountGroupId);

            if (accountGroup.Id == Guid.Empty)
                throw new NotFoundException("Account Group doesn't exist.");

            var role = await _userManager.GetRolesAsync(user);

            if (accountGroup.AdminUserId != currentUser)
                throw new ForbiddenException("You are not authorized to delete this account group");

            if (!role.Contains(StaticUserRoles.GROUPADMIN))
                throw new ForbiddenException("Only 'Group Admin' can delete account groups");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var deleteUser = await _context.Users.Where(u => u.Account.AccountGroupId == accountGroupId).ToListAsync();
                var deleteAccount = await _context.Accounts.Where(a => a.AccountGroupId == accountGroupId).ToListAsync();
                var deleteBudget = await _context.Budgets.Where(b => b.AccountGroupId == accountGroupId).ToListAsync();
                var deleteIncome = await _context.Incomes.Where(i => i.AccountGroupId == accountGroupId).ToListAsync();
                var deleteExpenses = await _context.Expenses.Where(e => e.AccountGroupId == accountGroupId).ToListAsync();

                _context.Budgets.RemoveRange(deleteBudget);
                _context.Incomes.RemoveRange(deleteIncome);
                _context.Expenses.RemoveRange(deleteExpenses);
                _context.Accounts.RemoveRange(deleteAccount);
                _context.AccountGroups.Remove(accountGroup);
                _context.Users.RemoveRange(deleteUser);

                var notification = new AddNotificationDto
                {
                    UserId = user.Id,
                    Type = StaticNotificationTypes.accountGroupDelete,
                    Message = $"{user.UserName}, have deleted their accountGroup.",
                    IsRead = false
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.NotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Account Group Deleted Successfully"
                };
            }
            catch(AuthException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error while deleting account group");
                throw new AuthException("An error occured while deleting account group", ex);
            }   
        }
    }
}