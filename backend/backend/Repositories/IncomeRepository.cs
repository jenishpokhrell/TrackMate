using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Model;
using backend.Model.Dto.Income;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Repositories
{
    public class IncomeRepository : IIncomeRepository
    {
        private readonly DapperContext _dbo;
        private readonly IUserContextService _userContext;

        public IncomeRepository(DapperContext dbo, IUserContextService userContext)
        {
            _dbo = dbo;
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        public async Task AddIncome(Income income)
        {
            var query = "INSERT INTO Incomes (Id, UserId, AccountGroupId, Source, Amount, Description, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, CategoryId, IsDeleted) " +
                "VALUES (@Id, @UserId, @AccountGroupId, @Source, @Amount, @Description, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @CategoryId, @IsDeleted)";

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

        public async Task<decimal> GetTotalIncome(Guid accountGroupId)
        {
            var query = "SELECT ISNULL(SUM(Amount), 0) FROM Incomes WHERE AccountGroupId = @accountGroupId";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<decimal>(query, new { accountGroupId });
            }
        }

        public async Task UpdateIncome(UpdateIncomeDto updateIncomeDto, Guid Id)
        {
            var updatedAt = DateTime.Now;
            var updatedBy = _userContext.GetCurrentLoggedInUserUsername();

            var query = "UPDATE Incomes SET Source = @Source, Amount = @Amount, Description = @Description, CategoryId = @CategoryId, UpdatedAt = @UpdatedAt, " +
                "UpdatedBy = @UpdatedBy WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.Guid);
            parameters.Add("Source", updateIncomeDto.Source, DbType.String);
            parameters.Add("Amount", updateIncomeDto.Amount, DbType.Decimal);
            parameters.Add("Description", updateIncomeDto.Description, DbType.String);
            parameters.Add("CategoryId", updateIncomeDto.CategoryId, DbType.Guid);
            parameters.Add("UpdatedAt", updatedAt, DbType.DateTime);
            parameters.Add("UpdatedBy", updatedBy, DbType.String);

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
