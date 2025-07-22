using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class ApplicationUser : IdentityUser
    {
        public Account Account { get; set; }
        public string Role { get; set; }
    }
}