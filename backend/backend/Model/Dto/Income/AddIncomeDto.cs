using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Income
{
    public sealed record AddIncomeDto
    {
        public string Source { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; }
        public Guid CategoryId { get; init; }
    }
}
