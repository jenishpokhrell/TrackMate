﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class Income : AuditableEntity
    {
        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public Guid? AccountGroupId { get; set; }
        public AccountGroup AccountGroup { get; set; }

        public string Source { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}
