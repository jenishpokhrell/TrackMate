using backend.Model.Dto.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddWelcomeNotificationAsync();
    }
}
