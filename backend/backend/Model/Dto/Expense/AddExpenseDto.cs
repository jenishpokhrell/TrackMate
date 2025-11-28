using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Expense
{
    public sealed record AddExpenseDto
    {
        public string Title { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; }
        public Guid CategoryId { get; init; }
    };
}
