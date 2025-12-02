using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
 
    public class AccountGroup : BaseEntity
    {
        public string Name { get; set; }

        public Guid AccountTypeId { get; set; }
        public AccountType AccountType { get; set; }

        public string AdminUserId { get; set; }
        public ApplicationUser AdminUser { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Expense> Expenses { get; set; }  

    }
}
