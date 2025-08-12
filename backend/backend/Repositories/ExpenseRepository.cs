using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.DataContext;
using backend.Dto.Expense;
using backend.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DapperContext _dbo;

        public ExpenseRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }
        public async Task AddExpenses(Expense expense)
        {
            var query = "INSERT INTO Expenses (Id, Title, Amount, Description, CategoryId, UserId, AccountGroupId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted) " +
                "VALUES (@Id, @Title, @Amount, @Description, @CategoryId, @UserId, @AccountGroupId, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @IsDeleted)";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, expense);
            }
        }

        public async Task<decimal> GetTotalExpense(Guid accountGroupId)
        {
            var query = "SELECT ISNULL(SUM(Amount), 0) AS TotalExpenses FROM Expenses WHERE AccountGroupId = @AccountGroupId";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<decimal>(query, new { AccountGroupId = accountGroupId });
            }
        }
    }
}
