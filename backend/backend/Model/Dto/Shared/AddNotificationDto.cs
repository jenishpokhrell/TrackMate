using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Shared
{
    public sealed record AddNotificationDto
    {
        public string UserId { get; init; }
        public string Type { get; init; }
        public string Message { get; init; }
        public bool IsRead { get; init; }
    }
}
