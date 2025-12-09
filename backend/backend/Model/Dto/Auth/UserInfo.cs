using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Auth
{
    public sealed record UserInfo
    {
        public string Id { get; set; }
        public string Email { get; init; }
        public string Name { get; init; }
        public string Username { get; init; }
        public string Address { get; init; }
        public string Contact { get; init; }
        public string Gender { get; init; }
        public string GroupName { get; init; }
        public string Roles { get; init; }
    };
}
