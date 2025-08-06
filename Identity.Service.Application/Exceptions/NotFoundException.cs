using System.Globalization;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions
{
    [Serializable]
    public class NotFoundException : IdentityException
    {
        public NotFoundException(string entity, object key)
            : base($"{entity} with key '{key}' was not found.") { }

        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
        protected NotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base("One or more errors occurred.")
        {
        }
    }
}