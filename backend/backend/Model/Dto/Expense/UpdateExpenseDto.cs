using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Expense
{
    public sealed record UpdateExpenseDto
    {
        public string Title { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; }
        public Guid CategoryId { get; init; }
        public string UpdatedBy { get; init; }
        public DateTime UpdateAt { get; init; }
    }
}
