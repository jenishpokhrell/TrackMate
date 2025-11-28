using backend.Core.Constants;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services;
using backend.DataContext;
using backend.Model;
using backend.Model.Dto.Shared;
using backend.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared
{
    public class NotificationService : INotificationService
    {
        private readonly IUserContextService _user;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUserContextService user, INotificationRepository notificationRepository, ILogger<NotificationService> logger)
        {
            _user = user;
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task<Notification> WelcomeNotificationAsync(AddNotificationDto addNotificationDto, DbTransaction transaction = null)
        {
            _logger.LogInformation("Welcome message for successful user registration.");
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = addNotificationDto.UserId,
                Type = addNotificationDto.Type,
                Message = addNotificationDto.Message,
                IsRead = addNotificationDto.IsRead,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddNotification(notification, transaction);
            _logger.LogInformation("Successfully stored notificaation for successful user registration.");
            return notification;
        }

        public async Task<Notification> NotificationAsync(AddNotificationDto addNotificationDto, DbTransaction transaction = null)
        {
            _logger.LogInformation("Message for successful notification...");
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = _user.GetCurrentLoggedInUserID(),
                Type = addNotificationDto.Type,
                Message = addNotificationDto.Message,
                IsRead = addNotificationDto.IsRead,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddNotification(notification, transaction);
            _logger.LogInformation("Successfully stored notification.");
            return notification;
        }

        /*public async Task ExpensesAddedNotificationAsync(AddNotificationDto addNotificationDto, DbTransaction transaction = null)
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

        public async Task IncomeAddedNotificationAsync()
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
        }*/
    }
}
