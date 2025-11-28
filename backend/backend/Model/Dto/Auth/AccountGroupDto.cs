using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record AccountGroupDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }

        public Guid AccountTypeId { get; init; }

        public string? AdminUserId { get; init; }
    };
}
