using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record LoginInfo
    {
        public string Id { get; set; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public string PhoneNumber { get; init; }
        public string Roles { get; init; }
    }
}
