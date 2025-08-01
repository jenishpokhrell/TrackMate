using backend.Core.Dto.GeneralDto;
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

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
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
    }
}
