using backend.Core.Dto.GeneralDto;
using backend.DataContext;
using backend.Exceptions;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Services.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class AccountCreationService : IAccountCreationService
    {
        private readonly ILogger<AccountCreationService> _logger;
        private readonly ApplicationDbContext _context;

        public AccountCreationService(ILogger<AccountCreationService> logger, ApplicationDbContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task DuoPersonAccountCreationAsync(AccountDto accountDto)
        {
            try
            {
                _logger.LogInformation("Creating Account for Duo Person");
                var createAccount = new Account
                {
                    Name = accountDto.Name,
                    Gender = accountDto.Gender,
                    Address = accountDto.Address,
                    AccountRole = accountDto.AccountRole,
                    UserId = accountDto.UserId,
                    AccountGroupId = accountDto.AccountGroupId
                };

                _context.Accounts.Add(createAccount);
                _logger.LogInformation("Account for {Name} created successfully. ", accountDto.Name);

                return Task.CompletedTask;
            }
            catch (Exception ex) when (!(ex is AccountNotFormedException))
            {
                _logger.LogError(ex, "Account creation for {Name} failed.", accountDto.Name);
                throw new AccountNotFormedException($"Failed to create account for '{accountDto.Name}'.", ex);
            }
        }

        public Task IndividualAccountCreationAsync(AccountDto accountDto)
        {
            try
            {
                _logger.LogInformation("Creating Account for Individual User");
                var createAccount = new Account
                {
                    Name = accountDto.Name,
                    Gender = accountDto.Gender,
                    Address = accountDto.Address,
                    AccountRole = null,
                    UserId = accountDto.UserId,
                    AccountGroupId = accountDto.AccountGroupId
                };

                _context.Accounts.Add(createAccount);
                //await _context.SaveChangesAsync();
                _logger.LogInformation("Account for {Name} created successfully.", accountDto.Name);

                return Task.CompletedTask;
                
            }
            catch (Exception ex) when (!(ex is AccountNotFormedException))
            {
                _logger.LogError(ex, "Account creation for {Name} failed.", accountDto.Name);
                throw new AccountNotFormedException($"Failed to create account for '{accountDto.Name}'.", ex);
            }
        }
    }
}
