using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Dto.Expense;
using backend.Model;
using backend.Services.Helpers;
using backend.Services.Shared.Interfaces.IExpense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.SharedExpense
{
    public class ExpenseCreationService : IExpenseCreationService
    {
        private readonly IUserContextService _userContext;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IFindAccountGroupId _findAccountGroupId;

        public ExpenseCreationService(IUserContextService userContext, IExpenseRepository expenseRepository, IFindAccountGroupId findAccountGroupId)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _expenseRepository = expenseRepository ?? throw new ArgumentNullException(nameof(expenseRepository));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
        }

        public async Task AddExpensesAsync(AddExpenseDto addExpenseDto)
        {
            var currentLoggedInUserId = _userContext.GetCurrentLoggedInUserID();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentLoggedInUserId);

            var currentLoggedInUsername = _userContext.GetCurrentLoggedInUserUsername();

            var expense = new Expense
            {
                Id = Guid.NewGuid(),
                Title = addExpenseDto.Title,
                Amount = addExpenseDto.Amount,
                Description = addExpenseDto.Description,
                CategoryId = addExpenseDto.CategoryId,
                UserId = currentLoggedInUserId,
                AccountGroupId = accountGroupId,
                CreatedBy = currentLoggedInUsername,
                UpdatedBy = currentLoggedInUsername,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };

            await _expenseRepository.AddExpenses(expense);
        }
    }
}
