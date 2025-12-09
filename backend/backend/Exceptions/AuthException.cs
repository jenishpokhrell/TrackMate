using backend.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public class AuthException : AppException
    {
        public AuthException(string message, Exception innerException) : base(message, innerException) { }

        public AuthException(string accountType) : base($"Account Type '{accountType}' not found") { }
    }
}
