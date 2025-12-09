using AutoMapper;
using backend.Core.Constants;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services.Shared;
using backend.DataContext;
using backend.Dto.Income;
using backend.Exceptions;
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
using System.ComponentModel.DataAnnotations;
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
                _logger.LogWarning("Attempted to add income with null data.");
                throw new ValidationException("Attempted income addition with null data.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("Initializing adding income...");

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
                    Message = "Your income has been added successfully.",
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
                    Message = "Income added successfully."
                };
            }
            catch (IncomeException ex)
            {
                await transaction.RollbackAsync();  
                _logger.LogError("Couldn't add income...");
                throw new IncomeException($"An error occured while adding income: {ex.Message}");
            }
        }

        public async Task<IEnumerable<GetIncomeDto>> GetIncomesAsync()
        {
            _logger.LogInformation("Getting all income data...");
            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();
            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentLoggedInUser);

            if (accountGroupId == Guid.Empty)
                throw new NotFoundException("Account Group not found.");

            var incomes = await _incomeRepository.GetIncomes(accountGroupId);

            return incomes.Any() == true 
                ? _mapper.Map<IEnumerable<GetIncomeDto>>(incomes)
                : throw new NotFoundException("You haven't added your income yet.");
        }

        public async Task<GetTotalIncomeDto> GetTotalIncomeAsync()
        {
            _logger.LogInformation("Getting total income data...");

            var currentUserId = _userContext.GetCurrentLoggedInUserID();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentUserId);

            if (accountGroupId == Guid.Empty)
                throw new NotFoundException("Account Group not found");

            var totalIncome = await _incomeRepository.GetTotalIncome(accountGroupId);

            return _mapper.Map<GetTotalIncomeDto>(totalIncome);
        }

        public async Task<GeneralServiceResponseDto> UpdateIncomeAsync(UpdateIncomeDto updateIncomeDto, Guid Id)
        {
            if(updateIncomeDto is null)
            {
                _logger.LogError("Updating data with null value.");
                throw new ValidationException("Updating data with null value.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            _logger.LogInformation("Initialization of updating income data...");

            var currentIncome = await _incomeRepository.GetIncomeById(Id);

            if (currentIncome is null)
                throw new NotFoundException("Income not found");

            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

            if (currentIncome.UserId != currentLoggedInUser)
                throw new ForbiddenException("You are not authorized to update this income");

            try
            {
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
            catch(IncomeException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Updating income failed", ex);
                throw new IncomeException($"Updating income failed: {ex.Message}");
            }
        }

        public async Task<GeneralServiceResponseDto> DeleteIncomesAsync(Guid Id)
        {
            var currentIncome = await _incomeRepository.GetIncomeById(Id);

            if(currentIncome is null)
                throw new NotFoundException("Income not found");

            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

            if(currentIncome.UserId != currentLoggedInUser)
                throw new ForbiddenException("You are not authorized to delete this income");

            await _incomeRepository.DeleteIncome(Id);

            return new GeneralServiceResponseDto
            {
                Success = true,
                StatusCode = 200,
                Message = "Income has been deleted successfully."
            };
        }
    }
}
