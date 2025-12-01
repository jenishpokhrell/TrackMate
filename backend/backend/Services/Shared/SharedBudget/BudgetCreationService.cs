using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Model;
using backend.Services.Helpers;
using backend.Services.Shared.Interfaces.IBudget;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.SharedBudget
{
    public class BudgetCreationService : IBudgetCreationService
    {
        private readonly ILogger<BudgetCreationService> _logger;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IUserContextService _userContext;
        private readonly ApplicationDbContext _context;
        private readonly IFindAccountGroupId _findAccountGroupId;

        public BudgetCreationService(ILogger<BudgetCreationService> logger, IBudgetRepository budgetRepository, IUserContextService userContext, ApplicationDbContext context, IFindAccountGroupId findAccountGroupId)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _budgetRepository = budgetRepository ?? throw new ArgumentNullException(nameof(budgetRepository));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
        }

        public async Task AddBudgetAsync(AddBudgetDto budgetDto)
        {
            try
            {
                var currentLoggedInUserId = _userContext.GetCurrentLoggedInUserID();

                var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentLoggedInUserId);

                var name = await _context.Accounts.Where(a => a.UserId == currentLoggedInUserId).Select(a => a.Name).FirstOrDefaultAsync();

                var budget = new Budget
                {
                    UserId = currentLoggedInUserId,
                    Amount = budgetDto.Amount,
                    AccountGroupId = accountGroupId,
                    CreatedBy = name,
                };

                _context.Budgets.Add(budget);
                _logger.LogInformation("Budget added successfully.");
            }
            catch(Exception ex)
            {
                _logger.LogError("Error while adding budget", ex);
                throw new Exception("Error while adding budget", ex);
            }
        }
    }
}
