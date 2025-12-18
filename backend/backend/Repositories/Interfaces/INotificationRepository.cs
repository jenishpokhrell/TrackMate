using backend.Model;
using backend.Model.Dto.Shared;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface INotificationRepository
    {
        Task AddNotification(Notification notification, DbTransaction transaction);
        Task<IEnumerable<Notification>> GetMyNotifications(string userId);
    }
}
