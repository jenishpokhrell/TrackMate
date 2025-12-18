using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Shared
{
    public sealed record GetNotificationDto
    {
        public string Type { get; init; }
        public string Message { get; init; }
    }
}
