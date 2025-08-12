using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend
{
    public class UserRegistrationException : Exception
    {
        public UserRegistrationException(string message) : base(message) { }

        public UserRegistrationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
