using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public class AccountTypeNotFoundException : Exception
    {
        public AccountTypeNotFoundException(string accountType) : base($"Account Type '{accountType}' not found") { }
    }
}
