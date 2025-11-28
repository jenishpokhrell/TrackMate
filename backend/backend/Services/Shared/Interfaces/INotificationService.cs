using backend.Model;
using backend.Model.Dto.Shared;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> WelcomeNotificationAsync(AddNotificationDto notificationDto, DbTransaction transcation);

        Task<Notification> NotificationAsync(AddNotificationDto notificationDto, DbTransaction transcation);

        /*Task IncomeAddedNotificationAsync();

        Task ExpensesAddedNotificationAsync();*/
    }
}
