using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Auth
{
    public class UserInfo
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public string Contact { get; set; }
        public string Gender { get; set; }
        public string GroupName { get; set; }
        public List<string> Roles { get; set; }
    }
}
