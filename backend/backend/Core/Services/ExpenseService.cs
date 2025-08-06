using AutoMapper;
using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Expense;
using backend.Model;
using Microsoft.EntityFrameworkCore;
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
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ExpenseService(IExpenseRepository expenseRepository, IMapper mapper, ApplicationDbContext context)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
            _context = context;
        }
        public async Task<GeneralServiceResponseDto> AddExpensesAsync(ClaimsPrincipal User, AddExpenseDto addExpenseDto)
        {
            try
            {
                var currentLoggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var accountGroup = await _context.Accounts.Where(a => a.UserId == currentLoggedInUser).Select(a => a.AccountGroupId).FirstOrDefaultAsync();

                var currentLoggedInUsername = User.FindFirstValue(ClaimTypes.Name);

                var expense = new Expense
                {
                    Id = Guid.NewGuid(),
                    Title = addExpenseDto.Title,
                    Amount = addExpenseDto.Amount,
                    Description = addExpenseDto.Description,
                    CategoryId = addExpenseDto.CategoryId,
                    UserId = currentLoggedInUser,
                    AccountGroupId = accountGroup,
                    CreatedBy = currentLoggedInUsername,
                    UpdatedBy = currentLoggedInUsername,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDeleted = false
                };

                var notification = new Notification
                {
                    UserId = currentLoggedInUser,
                    Type = "Expenses Added",
                    Message = $"{currentLoggedInUsername}, added an expenses recently.",
                    IsRead = false
                };

                await _expenseRepository.AddExpenses(expense);
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return new GeneralServiceResponseDto()
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Expenses added successfully."
                };
            }
            catch (Exception ex)
            {
                return new GeneralServiceResponseDto()
                {
                    Success = false,
                    StatusCode = 500,
                    Message = $"Failed to add expenses: {ex.Message}."
                };
            }
        }

        public async Task<GetTotalExpensesDto> GetMyTotalExpensesAsync(ClaimsPrincipal User)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var accountGroupId = await _context.Accounts.Where(a => a.UserId == userId).Select(a => a.AccountGroupId).FirstOrDefaultAsync();

            if(accountGroupId == Guid.Empty)
            {
                throw new Exception("AccountGroupId not found");
            }

            var totalExpense = await _expenseRepository.GetTotalExpense(accountGroupId);

            return _mapper.Map<GetTotalExpensesDto>(totalExpense);
        }
    }
}
