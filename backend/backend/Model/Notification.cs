using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class Notification : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
    }
}
