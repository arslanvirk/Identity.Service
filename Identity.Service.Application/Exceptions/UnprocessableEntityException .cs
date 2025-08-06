using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions
{
    [Serializable]
    public class UnprocessableEntityException : Exception
    {
        public UnprocessableEntityException() : base("UnProcessable Entity")
        {
        }
        public UnprocessableEntityException(Dictionary<string, string[]> dictionary) : base("One or more validation failures occurred.")
        {
        }

        public UnprocessableEntityException(string message) : base(message)
        {
        }
        public UnprocessableEntityException(string message, params object[] args) : 
            base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
        protected UnprocessableEntityException(SerializationInfo serializationInfo, 
            StreamingContext streamingContext)
            : base("UnProcessable Entity")
        {
        }
    }
}
