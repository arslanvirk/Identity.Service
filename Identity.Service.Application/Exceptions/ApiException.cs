using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions
{

    [Serializable]
    public class ApiException : Exception
    {
        public ApiException() : base("One or more errors occurred.")
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
        protected ApiException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base("One or more errors occurred.")
        {
        }
    }
}
