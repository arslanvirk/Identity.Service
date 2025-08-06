using System.Globalization;
using System.Runtime.Serialization;

namespace Identity.Service.Application.Exceptions;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException() : base()
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }

    public BadRequestException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
    protected BadRequestException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        : base("One or more errors occurred.")
    {
    }
}