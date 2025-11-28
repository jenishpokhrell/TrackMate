using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public class GroupNotFormedException : Exception
    {
        public GroupNotFormedException () { }

        public GroupNotFormedException (string message) : base(message) { }

        public GroupNotFormedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
