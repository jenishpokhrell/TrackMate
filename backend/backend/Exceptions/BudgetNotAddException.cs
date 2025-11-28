using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class BudgetNotAddException : Exception
    {
        public BudgetNotAddException () { }

        public BudgetNotAddException (string message) : base(message) { }

        public BudgetNotAddException (string message, Exception innerException) : base(message, innerException) { }
    }
}
