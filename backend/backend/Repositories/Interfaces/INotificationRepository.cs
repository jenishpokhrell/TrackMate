using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface INotificationRepository
    {
        Task AddNotification(Notification notification);
    }
}
