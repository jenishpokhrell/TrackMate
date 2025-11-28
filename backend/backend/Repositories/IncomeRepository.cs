using backend.Core.Interfaces.IRepositories;
using backend.DataContext;
using backend.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly DapperContext _dbo;

        public IncomeRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }
        public async Task AddIncome(Income income)
        {
            var query = "INSERT INTO Incomes (Id, UserId, AccountGroupId, Source, Amount, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted) " +
                "VALUES (@Id, @UserId, @AccountGroupId, @Source, @Amount, @Description, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @IsDeleted)";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, income);
            }
        }

        public async Task<Income> GetIncomeById(Guid Id)
        {
            var query = "SELECT * FROM Income WHERE Id = @Id";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Income>(query, new { Id });
            }
        }
    }
}
