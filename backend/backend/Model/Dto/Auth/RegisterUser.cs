using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record RegisterUser
    {
        public string Email { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public string Name { get; init; }
        public Gender Gender { get; init; }
        public string Address { get; init; }
        public string PhoneNumber { get; init; }
        public string GroupName { get; init; }
    }
}
