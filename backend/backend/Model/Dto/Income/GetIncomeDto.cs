using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Income
{
    public sealed record GetIncomeDto
    {
        public Guid CategoryId { get; init; }
        public string Source { get; init; }
        public decimal Amount { get; init; }
        public string Description { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string CreatedBy { get; init; }
        public string UpdatedBy { get; init; }
        
    }
}
