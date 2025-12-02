using backend.Constants;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.DataContext;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Services.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class GroupCreationService : IGroupCreationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GroupCreationService> _logger;

        public GroupCreationService(ILogger<GroupCreationService> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<AccountGroup> CreateDuoGroupAsync(AccountGroupDto accountGroupDto)
        {
            try
            {
                _logger.LogInformation("Creating Account Group for Duo Account Type");
                var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Type == StaticAccountType.DUO);

                if (accountType is null)
                {
                    throw new GroupNotFormedException("Duo account type not found");
                }

                var group = new AccountGroup
                {
                    Name = accountGroupDto.Name,
                    AdminUserId = accountGroupDto.AdminUserId,
                    AccountTypeId = accountType.Id
                };

                _context.AccountGroups.Add(group);
                _logger.LogInformation("Account Group for {GroupName} created successfully.", accountGroupDto.Name);

                return group;

            }
            catch (Exception ex) when (!(ex is GroupNotFormedException))
            {
                _logger.LogError(ex, "Account Group creation for {GroupName} failed.", accountGroupDto.Name);
                throw new GroupNotFormedException($"Failed to create account group for '{accountGroupDto.Name}'.", ex);
            }
        }

        public async Task<AccountGroup> CreateIndividualGroupAsync(AccountGroupDto accountGroupDto)
        {
            try
            {
                _logger.LogInformation("Creating Account Group for Individual Account Type");
                var accountType = await _context.AccountTypes.FirstOrDefaultAsync(a => a.Type == StaticAccountType.INDIVIDUAL);

                if(accountType is null)
                {
                    throw new GroupNotFormedException("Duo account type not found");   
                }

                var group = new AccountGroup
                {
                    Name = accountGroupDto.Name,
                    AdminUserId = accountGroupDto.AdminUserId,
                    AccountTypeId = accountType.Id
                };

                _context.AccountGroups.Add(group);
                _logger.LogInformation("Account Group for {GroupName} created successfully.", accountGroupDto.Name);

                return group;
                
            } catch(Exception ex) when (!(ex is GroupNotFormedException))
            {
                _logger.LogError(ex, "Account Group creation for {GroupName} failed.", accountGroupDto.Name);
                throw new GroupNotFormedException($"Failed to create account group for '{accountGroupDto.Name}'.", ex);
            }
        }
    }
}
