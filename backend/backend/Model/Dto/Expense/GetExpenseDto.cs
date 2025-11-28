using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Expense
{
    public sealed record GetExpenseDto
    {
        public Guid CategoryId { get; init; }
        public string Title { get; init; }
        public decimal Amount { get; init; }
        public string CreatedBy { get; init; }
        public string UpdatedBy { get; init; }
        public string Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
