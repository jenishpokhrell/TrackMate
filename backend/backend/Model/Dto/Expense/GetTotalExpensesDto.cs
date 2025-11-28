using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Expense
{
    public sealed record GetTotalExpensesDto
    {
        public decimal TotalExpenses { get; init; }
    };
}
