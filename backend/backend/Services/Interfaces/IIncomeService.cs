using backend.Core.Dto.GeneralDto;
using backend.Dto.Income;
using backend.Model.Dto.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IIncomeService
    {
        Task<GeneralServiceResponseDto> AddIncomeAsync(AddIncomeDto addIncomeDto);

        Task<GetTotalIncomeDto> GetTotalIncomeAsync();

        Task<GeneralServiceResponseDto> UpdateIncomeAsync(UpdateIncomeDto updateIncomeDto);
    }
}
