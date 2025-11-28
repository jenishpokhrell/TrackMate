using backend.Model;
using backend.Model.Dto.Income;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface IIncomeRepository
    {
        Task AddIncome(Income income);

        Task<Income> GetIncomeById(Guid Id);
    }
}
