using System.Globalization;
using System.Runtime.Serialization;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Application.Exceptions
{
    [Serializable]
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base(PermissionError)
        {
        }

        public ForbiddenException(string message) : base(message)
        {
        }


        public ForbiddenException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
        protected ForbiddenException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base("One or more errors occurred.")
        {
        }
    }
}
