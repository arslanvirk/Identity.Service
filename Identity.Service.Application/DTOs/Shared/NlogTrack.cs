using Microsoft.Extensions.Logging;
using NLog;

namespace Identity.Service.Application.DTOs.Shared
{
    public class NLogTrack
    {
        public required Logger Logger { get; set; }
        public string? TraceIdentifier { get; set; } = "Undefined";
        public string? CookieUserId { get; set; } = "Undefined";

        private string FormatMessage(string message) =>
            $"TraceId={TraceIdentifier}|CookieUserId={CookieUserId}|{message}";

        private Logger GetLoggerOrThrow()
        {
            if (Logger == null)
                throw new InvalidOperationException("Logger must be initialized before use.");
            return Logger;
        }

        public string GetLogMessage(string message) => FormatMessage(message);

        public void Info(string message) => GetLoggerOrThrow().Info(FormatMessage(message));
        public void Warn(string message) => GetLoggerOrThrow().Warn(FormatMessage(message));
        public void Error(string message) => GetLoggerOrThrow().Error(FormatMessage(message));
    }
}
