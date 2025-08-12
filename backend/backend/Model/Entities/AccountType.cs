using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class AccountType : BaseEntity
    {
        public string Type { get; set; }
        public int MaxAccounts { get; set; }

        public ICollection<AccountGroup> AccountGroups { get; set; }
    }
}
