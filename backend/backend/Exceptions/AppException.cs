using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }

        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }

    /*public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message) { }
    }*/

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
