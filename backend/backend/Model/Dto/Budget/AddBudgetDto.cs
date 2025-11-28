using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Budget
{
    public sealed record AddBudgetDto
    {
       public decimal Amount { get; init; }
    };
}
