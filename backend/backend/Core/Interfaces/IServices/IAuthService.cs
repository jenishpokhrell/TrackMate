using backend.Core.Dto.GeneralDto;
using backend.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterIndividualDto individualDto);
        Task<GeneralServiceResponseDto> RegisterDuoPerson1Async(RegisterDuoPerson1Dto person1Dto);
        Task<GeneralServiceResponseDto> RegisterDuoPerson2Async(RegisterDuoPerson2Dto person2Dto);
    }
}
