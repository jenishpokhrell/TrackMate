using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class BudgetException : AppException
    {
        public BudgetException (string message) : base(message) { }

        public BudgetException (string message, Exception innerException) : base(message, innerException) { }
    }
}
