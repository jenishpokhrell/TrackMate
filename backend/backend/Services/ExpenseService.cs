using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Expense;
using backend.Exceptions;
using backend.Model;
using backend.Model.Dto.Expense;
using backend.Model.Dto.Shared;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces.IExpense;
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
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserContextService _userContext;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExpenseService> _logger;
        private readonly IFindAccountGroupId _findAccountGroupId;
        private readonly IExpenseCreationService _expenseCreation;

        public ExpenseService(IExpenseRepository expenseRepository, INotificationService notificationService, IUserContextService userContext, IMapper mapper, 
            ApplicationDbContext context, ILogger<ExpenseService> logger, IFindAccountGroupId findAccountGroupId, IExpenseCreationService expenseCreation)
        {
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
            _expenseCreation = expenseCreation ?? throw new ArgumentNullException(nameof(expenseCreation));
        }
        public async Task<GeneralServiceResponseDto> AddExpensesAsync(AddExpenseDto addExpenseDto)
        {
            if (addExpenseDto is null)
            {
                _logger.LogWarning("Attempted to add expense with null data.");
                throw new ValidationException("Attempted Expense addition with null data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Initializing adding expenses...");

                var expense = new AddExpenseDto
                {
                    Title = addExpenseDto.Title,
                    Amount = addExpenseDto.Amount,
                    Description = addExpenseDto.Description,
                    CategoryId = addExpenseDto.CategoryId,
                };

                await _expenseCreation.AddExpensesAsync(expense);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.expensesAdd,
                    Message = "Your expenses has been added successfully.",
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
                    Message = "Expenses added successfully."
                };
            }
            catch (ExpenseException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Error while adding expenses");
                throw new ExpenseException($"An error occured while adding expenses: {ex.Message}");
            }
        }


        public async Task<GetTotalExpensesDto> GetTotalExpensesAsync()
        {
            var userId = _userContext.GetCurrentLoggedInUserID();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(userId);

            if(accountGroupId == Guid.Empty)
            {
                throw new NotFoundException("AccountGroup not found");
            }

            var totalExpense = await _expenseRepository.GetTotalExpense(accountGroupId);

            return _mapper.Map<GetTotalExpensesDto>(totalExpense);
        }

        public async Task<IEnumerable<GetExpenseDto>> GetAllExpensesAsync()
        {
            _logger.LogInformation("Fetching all expenses for a user...");

            var userId = _userContext.GetCurrentLoggedInUserID();
            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(userId);

            if (accountGroupId == Guid.Empty)
                throw new NotFoundException("Account Group not found.");

            var expenses = await _expenseRepository.GetAllExpenses(accountGroupId);

            _logger.LogInformation("Successfully fetched expense data.");
            return expenses.Any() == true 
                ? _mapper.Map<IEnumerable<GetExpenseDto>>(expenses)
                : throw new NotFoundException("Expenses not found");
        }

        public async Task<GeneralServiceResponseDto> UpdateExpenseAsync(UpdateExpenseDto updateExpenseDto, Guid Id)
        {
            if(updateExpenseDto is null)
            {
                _logger.LogWarning("Attempted update of expense with null data...");
                throw new  ValidationException("Updating expense with null data.");
            }

            _logger.LogInformation("Initialization of updating expense...");

            using var transaction = await _context.Database.BeginTransactionAsync();

            var currentExpense = await _expenseRepository.GetExpenseById(Id);

            if (currentExpense is null)
                throw new NotFoundException("Couldn't access expense. It either doesn't exist or try again later.");

            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

            if (currentExpense.UserId != currentLoggedInUser)
                throw new ForbiddenException("You are not authorized to update this expense.");

            try
            {
                await _expenseRepository.UpdateExpense(updateExpenseDto, Id);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.expensesUpdate,
                    Message = "Your expense has been updated successfully.",
                    IsRead = false,
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.NotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successful update of expense data and addition of notification.");
                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Expense has been updated successfully."
                };
            }
            catch (ExpenseException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Updating expense failed");
                throw new ExpenseException($"Updating expense failed: {ex.Message}");
            }
        }

        public async Task<GeneralServiceResponseDto> DeleteExpenseAsync(Guid Id)
        {
            _logger.LogInformation("Initializing the deletion of expense...");

            var currentExpense = await _expenseRepository.GetExpenseById(Id);
            if(currentExpense is null)
                throw new NotFoundException("Expense not found.");

            var currentLoggedInUser =  _userContext.GetCurrentLoggedInUserID();
            if(currentExpense.UserId != currentLoggedInUser)
                throw new ForbiddenException("You're not authorized to delete this expense.");

            await _expenseRepository.DeleteExpense(Id);
            return new GeneralServiceResponseDto
            {
                 Success = true,
                 StatusCode = 200,
                 Message = "Expense has been deleted successfully."
            };
        }
    }
}
