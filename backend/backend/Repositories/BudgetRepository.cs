using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly DapperContext _dbo;
        private readonly IUserContextService _userContext;
        public BudgetRepository(DapperContext dbo, IUserContextService userContext)
        {
            _dbo = dbo ?? throw new ArgumentNullException(nameof(dbo));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task<Budget> GetBudgetById(Guid id)
        {
            var query = "SELECT * FROM Budgets WHERE Id = @Id";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Budget>(query, new { id });
            }
        }

        public async Task UpdateBudget(UpdateBudgetDto updateBudgetDto, Guid id)
        {
            var updatedAt = DateTime.Now;
            var updatedBy = _userContext.GetCurrentLoggedInUserUsername();

            var query = "UPDATE Budgets SET Amount = @Amount, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            parameters.Add("Amount", updateBudgetDto.Amount, DbType.Decimal);
            parameters.Add("UpdatedAt", updatedAt, DbType.DateTime);
            parameters.Add("UpdatedBy", updatedBy, DbType.String);

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteBudget(Guid id)
        {
            var query = "DELETE FROM Budgets WHERE Id = @Id";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { id });
            }
        }
    }
}
