using backend.Core.Dto.GeneralDto;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Dto.Expense;
using backend.Model;
using backend.Model.Dto.Expense;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly DapperContext _dbo;
        private readonly IUserContextService _userContext;

        public ExpenseRepository(DapperContext dbo, IUserContextService userContext)
        {
            _dbo = dbo ?? throw new ArgumentNullException(nameof(dbo));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
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

        public async Task<Expense> GetExpenseById(Guid Id)
        {
            var query = "SELECT * FROM Expenses WHERE Id = @Id";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Expense>(query, new { Id });
            }
        }

        public async Task<IEnumerable<Expense>> GetAllExpenses(Guid Id)
        {
            var query = "SELECT * FROM Expenses WHERE AccountGroupId = @Id";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.QueryAsync<Expense>(query, new { Id });
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

        public async Task UpdateExpense(UpdateExpenseDto updateExpenseDto, Guid Id)
        {
            var updatedAt = DateTime.Now;
            var updatedBy = _userContext.GetCurrentLoggedInUserUsername();

            var query = "UPDATE Budgets SET Title = @Title, Amount = @Amount, Description = @Description, CategoryId = @CategoryId, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.Guid);
            parameters.Add("Title", updateExpenseDto.Title, DbType.String);
            parameters.Add("Description", updateExpenseDto.Description, DbType.String);
            parameters.Add("Amount", updateExpenseDto.Amount, DbType.Decimal);
            parameters.Add("CategoryId", updateExpenseDto.CategoryId, DbType.Guid);
            parameters.Add("UpdatedAt", updatedAt, DbType.DateTime);
            parameters.Add("UpdatedBy", updatedBy, DbType.String);

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteExpense(Guid Id)
        {
            var query = "DELETE FROM Expenses WHERE Id = @Id";

            using (var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, new { Id });
            }
        }
    }
}
