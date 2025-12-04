using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class BudgetException : Exception
    {
        public BudgetException () { }

        public BudgetException (string message) : base(message) { }

        public BudgetException (string message, Exception innerException) : base(message, innerException) { }

        //public BudgetException(int code, string message, Exception innerException) : base(code, message, innerException) { }
    }
}
