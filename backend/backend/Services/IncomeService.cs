using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Income;
using backend.Model;
using backend.Model.Dto.Income;
using backend.Model.Dto.Shared;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces.IIncome;
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
    public class IncomeService : IIncomeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomeRepository _incomeRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserContextService _userContext;
        private readonly ILogger<IncomeService> _logger;
        private readonly IIncomeCreationService _incomeCreationService;
        private readonly IFindAccountGroupId _findAccountGroupId;
        private readonly IMapper _mapper;

        public IncomeService(ApplicationDbContext context, IIncomeRepository incomeRepository, INotificationService notificationService, IUserContextService userContext,
            ILogger<IncomeService> logger, IIncomeCreationService incomeCreationService, IFindAccountGroupId findAccountGroupId, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _incomeRepository = incomeRepository ?? throw new ArgumentNullException(nameof(incomeRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _incomeCreationService = incomeCreationService ?? throw new ArgumentNullException(nameof(incomeCreationService));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<GeneralServiceResponseDto> AddIncomeAsync(AddIncomeDto addIncomeDto)
        {
            if (addIncomeDto is null)
            {
                _logger.LogWarning("Attempted to add expense with null data.");
                return ErrorResponse.CreateErrorResponse(400, "Attempted Expense addition with null data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Initializing adding expenses...");

                var income = new AddIncomeDto
                {
                    Source = addIncomeDto.Source,
                    Amount = addIncomeDto.Amount,
                    Description = addIncomeDto.Description,
                    CategoryId = addIncomeDto.CategoryId,
                };

                await _incomeCreationService.AddIncomeAsync(income);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.incomeUpdate,
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

        public async Task<IEnumerable<GetIncomeDto>> GetIncomesAsync()
        {
            _logger.LogInformation("Getting all income data...");
            try
            {
                var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

                var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentLoggedInUser);

                var incomes = await _incomeRepository.GetIncomes(accountGroupId);

                if(incomes is null)
                {
                    throw new Exception("You haven't added your income yet.");
                }

                return _mapper.Map<IEnumerable<GetIncomeDto>>(incomes);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income data.");
                throw new Exception("Error While fetching income data", ex);
            }
        }

        public async Task<GetTotalIncomeDto> GetTotalIncomeAsync()
        {
            try
            {
                _logger.LogInformation("Getting total income data...");

                var currentUserId = _userContext.GetCurrentLoggedInUserID();

                var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentUserId);

                if(accountGroupId == Guid.Empty)
                {
                    _logger.LogError("Account Group Not Found.");
                    throw new Exception("Cannot find account group");
                }

                var totalIncome = await _incomeRepository.GetTotalIncome(accountGroupId);

                _logger.LogInformation("Successfully fetched total income.");
                return _mapper.Map<GetTotalIncomeDto>(totalIncome);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while fetching income data.");
                throw new Exception("Error While fetching income data", ex);
            }
        }

        public async Task<GeneralServiceResponseDto> UpdateIncomeAsync(UpdateIncomeDto updateIncomeDto, Guid Id)
        {
            if(updateIncomeDto is null)
            {
                _logger.LogError("Updating data with null value.");
                throw new Exception("Updating data with null value.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Initialization of updating income data...");

                var currentIncome = await _incomeRepository.GetIncomeById(Id);

                if(currentIncome is null)
                {
                    return ErrorResponse.CreateErrorResponse(404, "No income found");
                }

                var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

                if(currentIncome.UserId != currentLoggedInUser)
                {
                    return ErrorResponse.CreateErrorResponse(401, "You are not authorized to update this income");
                }

                await _incomeRepository.UpdateIncome(updateIncomeDto, Id);

                var notification = new AddNotificationDto
                {
                    Type = StaticNotificationTypes.incomeUpdate,
                    Message = "Your income has been updated successfully.",
                    IsRead = false,
                };

                var dbTransaction = transaction.GetDbTransaction();
                await _notificationService.NotificationAsync(notification, dbTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successful update of income data and addition of notification.");
                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Income has been updated successfully."
                };
            }
            catch(Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync();
                }
                catch
                {
                    _logger.LogWarning("Transaction already completed, skipping rollback.");
                }
                _logger.LogError("Updating income failed", ex);
                return ErrorResponse.CreateErrorResponse(400, $"Updating income failed: {ex.Message}");
            }
        }

        public async Task<GeneralServiceResponseDto> DeleteIncomesAsync(Guid Id)
        {
            _logger.LogInformation("Initialization of deleting income...");
            try
            {
                var currentIncome = await _incomeRepository.GetIncomeById(Id);

                if(currentIncome is null)
                {
                    return ErrorResponse.CreateErrorResponse(404, "No income found");
                }

                var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

                if(currentIncome.UserId != currentLoggedInUser)
                {
                    return ErrorResponse.CreateErrorResponse(401, "You are not authorized to delete this income");
                }

                await _incomeRepository.DeleteIncome(Id);

                _logger.LogInformation("Income Successfully Deleted.");

                return new GeneralServiceResponseDto
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Income has been deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Income Deletion failed.", ex);
                return ErrorResponse.CreateErrorResponse(400, "Income deletion failed.");
            }
        }
    }
}
