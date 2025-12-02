using backend.Core.Dto.GeneralDto;
using backend.Dto.Budget;
using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface IBudgetRepository
    {
        Task<Budget> GetBudgetById(Guid Id);

        Task<IEnumerable<Budget>> GetBudgets(Guid Id);

        Task UpdateBudget(UpdateBudgetDto updateBudgetDto, Guid Id);

        Task DeleteBudget(Guid Id);
    }
}
