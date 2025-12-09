using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class AccountException : AppException
    {
        public AccountException(string message) : base(message) { }

        public AccountException(string message, Exception innerException) : base(message, innerException) { }
    }
}
