using backend.Core.Dto.GeneralDto;
using backend.Dto.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IIncomeService
    {
        Task<GeneralServiceResponseDto> AddIncomeAsync(ClaimsPrincipal User, AddIncomeDto addIncomeDto);
    }
}
