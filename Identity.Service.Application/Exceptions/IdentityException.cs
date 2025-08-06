using System.Globalization;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions;

[Serializable]
public class IdentityException : Exception
{
    public IdentityException(string message) : base(message) { }

    public IdentityException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
    protected IdentityException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base("One or more errors occurred.")
    {
    }
}
