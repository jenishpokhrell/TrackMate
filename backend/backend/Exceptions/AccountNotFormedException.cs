using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class AccountNotFormedException : Exception
    {
        public AccountNotFormedException() { }

        public AccountNotFormedException(string message) : base(message) { }

        public AccountNotFormedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
