using backend.Dto.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces.IBudget
{
    public interface IBudgetCreationService
    {
        Task AddBudgetAsync(AddBudgetDto budgeDto);
    }
}
