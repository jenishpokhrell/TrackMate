using backend.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public class AccountGroupException : AppException
    {
        public AccountGroupException (string message) : base(message) { }

        public AccountGroupException(string message, Exception innerException) : base(message, innerException) { }
    }
}
