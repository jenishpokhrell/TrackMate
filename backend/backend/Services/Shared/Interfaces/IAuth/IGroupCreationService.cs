using backend.Core.Dto.GeneralDto;
using backend.Model;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces
{
    public interface IGroupCreationService
    {
        Task<AccountGroup> CreateIndividualGroupAsync(AccountGroupDto accountGroupDto);

        Task<AccountGroup> CreateDuoGroupAsync(AccountGroupDto accountGroupDto);
    }
}
