using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum AccountRole
    {
        Person1,
        Person2
    }
    public class Account : BaseEntity
    {
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public AccountRole? AccountRole { get; set; }
        public string Address { get; set; }
            
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Guid AccountGroupId { get; set; }
        public AccountGroup AccountGroup { get; set; }
    }
}
