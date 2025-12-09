using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class ExpenseException : AppException
    {
        public ExpenseException (string message) : base(message) { }

        public ExpenseException (string message, Exception innerException) : base(message, innerException) { }
    }
}
