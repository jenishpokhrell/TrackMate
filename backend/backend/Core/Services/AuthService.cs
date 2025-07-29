using AutoMapper;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Auth;
using backend.Helpers;
using backend.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly GenereteJWTToken _generateJWTToken;

        public AuthService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext context, 
            GenereteJWTToken generateJWTToken)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
            _generateJWTToken = generateJWTToken;
        }

        public async Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterIndividualDto individualDto)
        {
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

            _context.AccountGroups.Add(group);
            _context.Accounts.Add(account);
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

            var account = new Account
            {
                Name = person2Dto.Name,
                Gender = person2Dto.Gender,
                Address = person2Dto.Address,
                AccountRole = AccountRole.Person2,
                ApplicationUser = user,
                AccountGroupId = group.Id
            };

            _context.Accounts.Add(account);
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

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isPasswordCorrect)
            {
                throw new Exception("Incorrect Password");
            }

            var newToken = await _generateJWTToken.GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfo(user, roles);

            return new LoginServiceResponseDto
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }

        private UserInfo GenerateUserInfo(ApplicationUser User, IList<string> Roles)
        {
            var userInfo = _mapper.Map<UserInfo>(User);
            userInfo.Roles = Roles.ToList();
            return userInfo;
        }
    }
}
