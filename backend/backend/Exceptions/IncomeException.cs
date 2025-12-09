using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class IncomeException : AppException
    {
        public IncomeException(string message) : base(message) { }

        public IncomeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
