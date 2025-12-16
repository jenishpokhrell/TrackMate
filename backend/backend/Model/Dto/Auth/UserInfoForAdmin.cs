using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record UserInfoForAdmin
    {
        public string Id { get; set; }
        public string Email { get; init; }
        public string Name { get; init; }
        public string UserName { get; init; }
        public string Address { get; init; }
        public string PhoneNumber { get; init; }
        public string Gender { get; init; }
        public DateTime AccountCreatedAt { get; init; }
        public Guid AccountGroupId { get; init; }
        public string AccountGroupName { get; init; }
        public string AdminUserId { get; init; }
        public Guid AccountTypeId { get; init; }
        public string AccountType { get; init; }
        //public string Roles { get; init; }
    }
}
