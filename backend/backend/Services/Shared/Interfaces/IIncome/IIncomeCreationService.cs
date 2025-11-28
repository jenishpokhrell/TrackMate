using backend.Dto.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces.IIncome
{
    public interface IIncomeCreationService
    {
        Task AddIncomeAsync(AddIncomeDto addIncomeDto);
    }
}
