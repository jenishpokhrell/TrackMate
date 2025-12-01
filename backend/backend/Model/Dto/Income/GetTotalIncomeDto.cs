using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Income
{
    public sealed record GetTotalIncomeDto
    {
        public decimal TotalIncome { get; init; }
    }
}
