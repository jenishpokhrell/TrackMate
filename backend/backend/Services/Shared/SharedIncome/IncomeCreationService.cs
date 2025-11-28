using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Repositories;
using backend.Dto.Income;
using backend.Model;
using backend.Services.Helpers;
using backend.Services.Shared.Interfaces.IIncome;
using System;
using System.Threading.Tasks;

namespace backend.Services.Shared.SharedIncome
{
    public class IncomeCreationService : IIncomeCreationService
    {
        private readonly IUserContextService _userContext;
        private readonly IFindAccountGroupId _findAccountGroupId;
        private readonly IIncomeRepository _incomeRepository;

        public IncomeCreationService(IUserContextService userContext, IFindAccountGroupId findAccountGroupId, IIncomeRepository incomeRepository)
        {
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _findAccountGroupId = findAccountGroupId ?? throw new ArgumentNullException(nameof(findAccountGroupId));
            _incomeRepository = incomeRepository ?? throw new ArgumentNullException(nameof(incomeRepository));
        }

        public async Task AddIncomeAsync(AddIncomeDto addIncomeDto)
        {
            var currentLoggedInUser = _userContext.GetCurrentLoggedInUserID();

            var currentLoggedInUsername = _userContext.GetCurrentLoggedInUserUsername();

            var accountGroupId = await _findAccountGroupId.FindAccountGroupIdAsync(currentLoggedInUser);

            var income = new Income
            {
                Id = Guid.NewGuid(),
                UserId = currentLoggedInUser,
                AccountGroupId = accountGroupId,
                Source = addIncomeDto.Source,
                Amount = addIncomeDto.Amount,
                Description = addIncomeDto.Description,
                CategoryId = addIncomeDto.CategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CreatedBy = currentLoggedInUsername,
                UpdatedBy = currentLoggedInUsername,
                IsDeleted = false
            };

            await _incomeRepository.AddIncome(income);
        }
    }
}
