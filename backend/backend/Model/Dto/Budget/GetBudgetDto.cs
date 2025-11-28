using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Budget
{
    public sealed record GetBudgetDto
    {
        public Guid Id { get; init; }
        public decimal Amount { get; init; }
        public bool IsExceeded { get; init; }
    };
}
