using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public Guid AccountGroupId { get; set; }
        public AccountGroup AccountGroup { get; set; }
        public ICollection<Expense> Expenses { get; set; }
    }
}
