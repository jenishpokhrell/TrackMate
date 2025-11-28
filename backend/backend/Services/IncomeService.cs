using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Income;
using backend.Model;
using backend.Model.Dto.Shared;
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

        public IncomeService(ApplicationDbContext context, IIncomeRepository incomeRepository, INotificationService notificationService, IUserContextService userContext,
            ILogger<IncomeService> logger, IIncomeCreationService incomeCreationService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _incomeRepository = incomeRepository ?? throw new ArgumentNullException(nameof(incomeRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _incomeCreationService = incomeCreationService ?? throw new ArgumentNullException(nameof(incomeCreationService));
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
    }
}
