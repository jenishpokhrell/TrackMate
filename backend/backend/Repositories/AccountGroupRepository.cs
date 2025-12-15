using backend.DataContext;
using backend.Model;
using backend.Repositories.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class AccountGroupRepository : IAccountGroupRepository
    {
        private readonly DapperContext _dbo;

        public AccountGroupRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }
        public async Task<IEnumerable<AccountGroup>> GetAllAccountGroups()
        {
            var query = "SELECT * FROM AccountGroups";

            using(var connection = _dbo.CreateConnection())
            {
                return await connection.QueryAsync<AccountGroup>(query);
            }
        }
    }
}
