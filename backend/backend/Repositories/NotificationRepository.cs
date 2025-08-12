using backend.Core.Interfaces.IRepositories;
using backend.DataContext;
using backend.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DapperContext _dbo;

        public NotificationRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }

        public async Task AddNotification(Notification notification)
        {
            var query = "INSERT INTO Notifications (Id, UserId, Type, Message, IsRead, CreatedAt, UpdatedAt) " +
                "VALUES (@Id, @UserId, @Type, @Message, @IsRead, @CreatedAt, @UpdatedAt)";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, notification);
            }
        }
    }
}
