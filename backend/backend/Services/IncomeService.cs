using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Income;
using backend.Model;
using Microsoft.EntityFrameworkCore;
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

        public IncomeService(ApplicationDbContext context, IIncomeRepository incomeRepository)
        {
            _context = context;
            _incomeRepository = incomeRepository;
        }
        public async Task<GeneralServiceResponseDto> AddIncomeAsync(ClaimsPrincipal User, AddIncomeDto addIncomeDto)
        {
            var currentLoggedInUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var currentLoggedInUsername = User.FindFirstValue(ClaimTypes.Name);

            var accountGroupId = await _context.Accounts.Where(a => a.UserId == currentLoggedInUser).Select(a => a.AccountGroupId).FirstOrDefaultAsync();

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

            var notification = new Notification
            {
                UserId = currentLoggedInUser,
                Type = "Income Added",
                Message = $"{currentLoggedInUsername}, added an income.",
                IsRead = false
            };

            
        }
    }
}
