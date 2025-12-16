using backend.DataContext;
using backend.Dto.Auth;
using backend.Model;
using backend.Model.Dto.Auth;
using backend.Repositories.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DapperContext _dbo;
        public AuthRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }

        public async Task<IEnumerable<UserInfoForAdmin>> GetAllAccounts()
        {
            var query = "SELECT u.Id, u.UserName, u.Email, u.PhoneNumber, a.Name, a.Gender, a.Address, ag.Id AS AccountGroupId, " +
                "ag.Name AS AccountGroupName, ag.AdminUserId, ag.CreatedAt AS AccountCreatedAt, at.Id AS AccountTypeId, at.Type AS AccountType " +
                "FROM Users u " +
                "INNER JOIN Accounts AS a ON u.Id = a.UserId " +
                "INNER JOIN AccountGroups AS ag ON a.AccountGroupId = ag.Id " +
                "INNER JOIN AccountTypes AS at ON ag.AccountTypeId = at.Id";

            using (var connection = _dbo.CreateConnection())
            {
                return await connection.QueryAsync<UserInfoForAdmin>(query);
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsers()
        {
            var query = "SELECT * FROM Users";

            using (var connection = _dbo.CreateConnection())
            {
                return await connection.QueryAsync<ApplicationUser>(query);
            }
        }

        public async Task GetUserById(string UserId)
        {
            var query = "SELECT * FROM Users WHERE Id = @UserId";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.QueryFirstOrDefaultAsync(query, new { UserId });
            }
        }

        public async Task UpdateAccount(Account account, string userId)
        {
            var query = "UPDATE Accounts SET Name = @Name, Gender = @Gender, Address = @Address WHERE UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.String);
            parameters.Add("Name", account.Name, DbType.String);
            parameters.Add("Gender", account.Gender, DbType.Int32);
            parameters.Add("Address", account.Address, DbType.String);

            using(var connection = _dbo.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
