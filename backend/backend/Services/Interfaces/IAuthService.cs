using backend.Core.Dto.GeneralDto;
using backend.Dto.Auth;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<GeneralServiceResponseDto> RegisterIndividualAsync(RegisterUser userDto);
        Task<GeneralServiceResponseDto> RegisterDuoPerson1Async(RegisterUser userDto);
        Task<GeneralServiceResponseDto> RegisterDuoPerson2Async(RegisterUser userDto);
        Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto);
    }
}
