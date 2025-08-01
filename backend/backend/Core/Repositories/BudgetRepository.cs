using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.DataContext;
using backend.Dto.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DapperContext _dbo;
        public BudgetRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }
        public Task<GeneralServiceResponseDto> AddBudget(ClaimsPrincipal User, AddBudgetDto addBudgetDto)
        {
            throw new NotImplementedException();
        }
    }
}
