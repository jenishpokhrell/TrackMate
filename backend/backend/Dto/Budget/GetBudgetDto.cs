using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Budget
{
    public class GetBudgetDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsExceeded { get; set; }
    }
}
