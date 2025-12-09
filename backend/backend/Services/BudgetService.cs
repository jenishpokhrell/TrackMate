using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Exceptions;
using backend.Model;
using backend.Model.Dto.Shared;
using backend.Services;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces.IBudget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;
        private IBudgetRepository _budgetRepository;
        private IExpenseRepository _expenseRepository;
        private IBudgetCreationService _budgetCreationService;
        private readonly IFindAccountGroupId _findAccountGroupId;
        private readonly IUserContextService _userContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BudgetService> _logger;
        private readonly IMapper _mapper;

        public BudgetService(ApplicationDbContext context, IMapper mapper, IBudgetRepository budgetRepository, IExpenseRepository expenseRepository, IBudgetCreationService budgetCreationService,
            IFindAccountGroupId findAccountGroupId, IUserContextService userContext, INotificationService notificationService, ILogger<BudgetService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _budgetRepository = budgetRepository ?? throw new ArgumentNullException(nameof(budgetRepository));
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _budgetCreationService = budgetCreationService ?? throw new ArgumentNullException(nameof(budgetCreationService));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService)) ;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GeneralServiceResponseDto> AddBudgetAsync(AddBudgetDto budgetDto)
        {
            if (budgetDto is null)
            {
                _logger.LogWarning("Attempted to add budget with null data.");
                throw new ValidationException("Attempted Budget addition with null data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var budget = new AddBudgetDto
                {
                    Amount = budgetDto.Amount
                };

                await _budgetCreationService.AddBudgetAsync(budget);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.budgetAdd,
                    Message = "Your budget has been added successfully.",
                    IsRead = false,
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.NotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponseDto
                {
                    StatusCode = 201,
                    Success = true,
                    Message = "Budget Added Successfully."
                };
            }
            catch (BudgetException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Budget adding failed.");
                throw new BudgetException("An unexpected error occured while adding the budget.", ex);
            }
        }

        public async Task<IEnumerable<GetBudgetDto>> GetAllBudgetsAsync()
        {
            _logger.LogInformation("Fetching all budgets for a user...");

            var userId = _userContext.GetCurrentLoggedInUserID();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(userId);

            if(accountGroupId == Guid.Empty)
                throw new NotFoundException("Account Group not found.");

            var budgets = await _budgetRepository.GetBudgets(accountGroupId);
           
            return budgets?.Any() == true
                ? _mapper.Map<IEnumerable<GetBudgetDto>>(budgets)
                : throw new NotFoundException("Budget Not Found");
        }

        public async Task<GetBudgetDto> GetMyRemainingBudgetAsync()
        {
            var userId = _userContext.GetCurrentLoggedInUserID();
            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(userId);

            if (accountGroupId == Guid.Empty)
                throw new NotFoundException("Account Group not found.");

            var totalExpense = await _expenseRepository.GetTotalExpense(accountGroupId);

            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.AccountGroupId == accountGroupId);

            if (budget is null)
                throw new NotFoundException("Budget not found.");

            var remainingBudget = budget.Amount - totalExpense;

            return new GetBudgetDto { Id = budget.Id, Amount = remainingBudget, IsExceeded = remainingBudget == 0 ? true : false };
        }

        public async Task<GeneralServiceResponseDto> UpdateBudgetAsync(UpdateBudgetDto updateBudgetDto, Guid id)
        {
            if (updateBudgetDto is null)
            {
                _logger.LogWarning("Attempted to update budget with null data.");
                return ErrorResponse.CreateErrorResponse(400, "Attempted Budget addition with null data.");
            }

            _logger.LogInformation("Initializing the update of budget...");
            using var transaction = await _context.Database.BeginTransactionAsync();

            var currentloggedInUserId = _userContext.GetCurrentLoggedInUserID();

            var budget = await _budgetRepository.GetBudgetById(id);

            if (budget is null)
                throw new NotFoundException("Budget not found.");

            if (budget.UserId != currentloggedInUserId)
                throw new ForbiddenException("You're not authorized to delete this budget.");

            try
            {
                await _budgetRepository.UpdateBudget(updateBudgetDto, id);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.budgetUpdate,
                    Message = "Your budget has been updated successfully.",
                    IsRead = false,
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.NotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Budget has been updated successfully."
                };
            }
            catch(BudgetException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Updating budget failed");
                throw new BudgetException("Error occured while updating budget", ex);
            }
        }

        public async Task<GeneralServiceResponseDto> DeleteBudgetAsync(ClaimsPrincipal User, Guid id)
        {
            _logger.LogInformation("Initializing the deletion of budget...");
            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

            var budget = await _budgetRepository.GetBudgetById(id);

            if (budget is null)
                throw new NotFoundException("Budget not found.");

            if (budget.UserId != currentLoggedInUser)
                throw new ForbiddenException("You are not authorized to delete this budget");

            await _budgetRepository.DeleteBudget(id);

            return new GeneralServiceResponseDto()
            {
                    Success = true,
                    StatusCode = 200,
                    Message = "Budget has been deleted successfully."
            };
        }

    }
}
