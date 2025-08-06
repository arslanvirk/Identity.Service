using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions
{
    [Serializable]
    public class RatelimitingException : Exception
    {
        public RatelimitingException() : base()
        {
        }

        public RatelimitingException(string message) : base(message)
        {
        }

        public RatelimitingException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
        protected RatelimitingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base("One or more errors occurred.")
        {
        }
    }
}
