using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Expense;
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
                return ErrorResponse.CreateErrorResponse(400, "Attempted Expense addition with null data.");
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
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch (InvalidOperationException)
                {
                    _logger.LogWarning("Transaction already completed, skipping rollback.");
                }
                return ErrorResponse.CreateErrorResponse(500, $"An error occured while adding expenses: {ex.Message}");
            }
        }


        public async Task<GetTotalExpensesDto> GetTotalExpensesAsync()
        {
            var userId = _userContext.GetCurrentLoggedInUserID();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(userId);

            if(accountGroupId == Guid.Empty)
            {
                throw new Exception("AccountGroupId not found");
            }

            var totalExpense = await _expenseRepository.GetTotalExpense(accountGroupId);

            return _mapper.Map<GetTotalExpensesDto>(totalExpense);
        }

        public async Task<GeneralServiceResponseDto> UpdateExpenseAsync(UpdateExpenseDto updateExpenseDto, Guid Id)
        {
            if(updateExpenseDto is null)
            {
                _logger.LogWarning("Attempted update of expense with null data...");
                return ErrorResponse.CreateErrorResponse(400, "Updating expense with null data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Initialization of updating expense...");

                var currentExpense = await _expenseRepository.GetExpenseById(Id);

                if(currentExpense is null)
                {
                    return ErrorResponse.CreateErrorResponse(404, "Couldn't access expense. It either doesn't exist or try again later.");
                }

                var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

                if(currentExpense.UserId != currentLoggedInUser)
                {
                    return ErrorResponse.CreateErrorResponse(401, "You are not authorized to update this expense.");
                }

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

                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Expense has been updated successfully."
                };
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch
                {
                    _logger.LogWarning("Transaction already completed, skipping rollback.");
                }
                _logger.LogError("Updating expense failed", ex);
                return ErrorResponse.CreateErrorResponse(400, $"Updating expense failed: {ex.Message}");
            }
        }

        public async Task<GeneralServiceResponseDto> DeleteExpenseAsync(Guid Id)
        {
            _logger.LogInformation("Initializing the deletion of expense...");
            try
            {
                var currentExpense = await _expenseRepository.GetExpenseById(Id);
                if(currentExpense is null)
                {
                    return ErrorResponse.CreateErrorResponse(404, "Expense doesn't exist.");
                }

                var currentLoggedInUser =  _userContext.GetCurrentLoggedInUserID();

                if(currentExpense.UserId != currentLoggedInUser)
                {
                    return ErrorResponse.CreateErrorResponse(401, "Expense doesn't exist.");
                }

                await _expenseRepository.DeleteExpense(Id);

                _logger.LogInformation("Expense Successfully Deleted.");

                return new GeneralServiceResponseDto
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Expense has been deleted successfully."
                };
            }
            catch(Exception ex)
            {
                _logger.LogError("Expense Deletion failed.", ex);
                return ErrorResponse.CreateErrorResponse(400, "Expense deletion failed.");
            }
        }
    }
}
