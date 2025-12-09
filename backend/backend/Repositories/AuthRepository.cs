using backend.DataContext;
using backend.Dto.Auth;
using backend.Repositories.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
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
        public async Task GetUserById(string UserId)
        {
            var query = "SELECT * FROM Users WHERE Id = @UserId";

            using(var connection = _dbo.CreateConnection())
            {
                await connection.QueryFirstOrDefaultAsync(query, new { UserId });
            }
        }
    }
}
