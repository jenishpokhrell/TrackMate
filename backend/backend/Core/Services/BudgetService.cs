using AutoMapper;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;
        private IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public BudgetService(ApplicationDbContext context, IMapper mapper, IBudgetRepository budgetRepository)
        {
            _context = context;
            _mapper = mapper;
            _budgetRepository = budgetRepository;
        }

        public async Task<GeneralServiceResponseDto> AddBudgetAsync(ClaimsPrincipal User, AddBudgetDto budgetDto)
        {
            var currentLoggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var accountGroupId = await _context.Accounts.Where(a => a.UserId == currentLoggedInUserId).Select(a => a.AccountGroupId).FirstOrDefaultAsync();

            var name = await _context.Accounts.Where(a => a.UserId == currentLoggedInUserId).Select(a => a.Name).FirstOrDefaultAsync();

            var budget = new Budget
            {
                UserId = currentLoggedInUserId,
                Amount = budgetDto.Amount,
                AccountGroupId= accountGroupId,
                CreatedBy = name,
            };

            var notification = new Notification
            {
                UserId = currentLoggedInUserId,
                Type = "Budget Added",
                Message = $"{name}, added a budget.",
                IsRead = false
            };

            _context.Budgets.Add(budget);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return new GeneralServiceResponseDto
            {
                StatusCode = 201,
                Success = true,
                Message = "Budget Added Successfully."
            };
        }

        public async Task<GetBudgetDto> GetMyBudgetAsync(ClaimsPrincipal User)
        {
            var currentLoggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budget = await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == currentLoggedInUser);

            if(budget is null)
            {
                throw new Exception("You haven't added your budget yet.");
            }

            return _mapper.Map<GetBudgetDto>(budget);
        }

        public async Task<GeneralServiceResponseDto> UpdateBudgetAsync(ClaimsPrincipal User, UpdateBudgetDto updateBudgetDto, Guid id)
        {
            var currentloggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budget = await _budgetRepository.GetBudgetById(id);

            if(budget is null)
            {
                return new GeneralServiceResponseDto()
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Budget not found."
                };
            }

            if(budget.UserId != currentloggedInUser)
            {
                return new GeneralServiceResponseDto()
                {
                    Success = false,
                    StatusCode = 401,
                    Message = "You're not authorized to update this budget."
                };
            }

            await _budgetRepository.UpdateBudget(User, updateBudgetDto, id);

            return new GeneralServiceResponseDto()
            {
                Success = true,
                StatusCode = 201,
                Message = "Budget has been updated successfully."
            };
        }

        public async Task<GeneralServiceResponseDto> DeleteBudgetAsync(ClaimsPrincipal User, Guid id)
        {
            var currentLoggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budget = await _budgetRepository.GetBudgetById(id);

            if (budget is null)
            {
                return new GeneralServiceResponseDto()
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "Budget not found."
                };
            }

            if(budget.UserId != currentLoggedInUser)
            {
                return new GeneralServiceResponseDto()
                {
                    Success = false,
                    StatusCode = 404,
                    Message = "You're not authorized to delete this budget."
                };
            }

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
