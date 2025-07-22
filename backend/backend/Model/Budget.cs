using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class Budget : AuditableEntity
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Guid? AccountGroupId { get; set; }
        public AccountGroup AccountGroup { get; set; }

        public decimal Amount { get; set; }
        public bool IsExceeded { get; set; }

    }
}
