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
    public class AccountTypeRepository : IAccountTypeRepository
    {
        private readonly DapperContext _dbo;

        public AccountTypeRepository(DapperContext dbo)
        {
            _dbo = dbo;
        }

        public async Task<IEnumerable<AccountType>> GetAllAccountType()
        {
            var query = "SELECT * FROM AccountTyes";

            using (var connection = _dbo.CreateConnection())
            {
                return await connection.QueryAsync<AccountType>(query);
            }
        }
    }
}
