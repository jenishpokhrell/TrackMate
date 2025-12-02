using backend.Core.Dto.GeneralDto;
using backend.Model;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces
{
    public interface IAccountCreationService
    {
        Task IndividualAccountCreationAsync(AccountDto accountDto);

        Task DuoPersonAccountCreationAsync(AccountDto accountDto);
    }
}
