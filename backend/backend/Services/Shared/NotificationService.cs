using AutoMapper;
using backend.Core.Constants;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services;
using backend.DataContext;
using backend.Exceptions;
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
        private readonly IUserContextService _userContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IUserContextService userContext, INotificationRepository notificationRepository, IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _userContext = userContext;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
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
                UserId = _userContext.GetCurrentLoggedInUserID(),
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

        public async Task<IEnumerable<GetNotificationDto>> GetMyNotificationsAsync()
        {
            var currentUser = _userContext.GetCurrentLoggedInUserID();

            var notifications = await _notificationRepository.GetMyNotifications(currentUser);

            if (notifications is null)
                throw new NotFoundException("You doesn't have any notifications yet.");

            return _mapper.Map<IEnumerable<GetNotificationDto>>(notifications);

        }
    }
}
