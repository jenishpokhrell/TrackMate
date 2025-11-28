using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record AccountDto
    {
        public string Name { get; init; }
        public Gender Gender { get; init; }
        public string Address { get; init; }
        public AccountRole AccountRole { get; init; }
        public string UserId { get; init; }
        public Guid AccountGroupId { get; init; }
    }
}
