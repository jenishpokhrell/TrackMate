using backend.Core.Constants;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Services;
using backend.DataContext;
using backend.Model;
using backend.Model.Dto.Shared;
using backend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class NotificationService : INotificationService
    {
        private readonly UserContextService _user;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(UserContextService user, INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _user = user;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task AddWelcomeNotificationAsync()
        {
            _logger.LogInformation("Welcome message for successful user registration.")
            var notification = new Notification
            {
                UserId = _user.GetCurrentLoggedInUserID(),
                Type = StaticNotificationTypes.welcome,
                Message = $"Welcome, {_user.GetCurrentLoggedInUserUsername()}. You have successfully created your account.",
                IsRead = false
            };

            await _notificationRepository.AddNotification(notification);
            return;
        }

        public async Task AddBudgetAddedNotificationAsync()
        {
            var notification = new Notification
            {
                UserId = _user.GetCurrentLoggedInUserID(),
                Type = StaticNotificationTypes.budget,
                Message = $"{_user.GetCurrentLoggedInUserUsername()} added a budget.",
                IsRead = false
            };

            await _notificationRepository.AddNotification(notification);
            return;
        }

        public async Task AddExpensesAddedNotificationAsync()
        {
            var notification = new Notification
            {
                UserId = _user.GetCurrentLoggedInUserID(),
                Type = StaticNotificationTypes.expenses,
                Message = $"{_user.GetCurrentLoggedInUserUsername()}, added an expense.",
                IsRead = false
            };

            await _notificationRepository.AddNotification(notification);
            return;
        }

        public async Task AddIncomeAddedNotificationAsync()
        {
            var notification = new Notification
            {
                UserId = _user.GetCurrentLoggedInUserID(),
                Type = StaticNotificationTypes.income,
                Message = $"{_user.GetCurrentLoggedInUserUsername()} added an income.",
                IsRead = false
            };

            await _notificationRepository.AddNotification(notification);
            return;
        }
    }
}
