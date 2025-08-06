using System.Net;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;
using Identity.Service.Application.Wrappers;
using Identity.Service.Application.Constants;
using Identity.Service.Application.Exceptions;
using Identity.Service.Application.DTOs.Shared;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Web.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Logger _logger;
        private static readonly JsonSerializerOptions CachedJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = LogManager.GetLogger("ErrorHandlerMiddleware");
        }

        public async Task Invoke(HttpContext context)
        {
            IServiceProvider services = context.RequestServices;

            NLogTrack nlogTrack = services.GetRequiredService<NLogTrack>();
            try
            {
                await _next(context);
                LogInfo(nlogTrack, ApiLogsSuccess);
            }
            catch (Exception error)
            {
                await HandleExceptionAsync(context, error, nlogTrack);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception error, NLogTrack nlogTrack)
        {
            LogException(nlogTrack, error);

            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = ResolveStatusCode(error);

            var responseModel = new ErrorResponse<string>
            {
                Success = false,
                Message = error.Message
            };

            LogErrorDetails(nlogTrack, response.StatusCode, error);
            LogInfo(nlogTrack, ApiLogsFail);

            var result = JsonSerializer.Serialize(responseModel, CachedJsonSerializerOptions);
            await response.WriteAsync(result);
        }

        private static int ResolveStatusCode(Exception error) =>
          error switch
          {
              ApiException => (int)HttpStatusCode.BadRequest,
              KeyNotFoundException => (int)HttpStatusCode.NotFound,
              NotSupportedException => (int)HttpStatusCode.NotAcceptable,
              UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
              ForbiddenException => (int)HttpStatusCode.Forbidden,
              UnprocessableEntityException => (int)HttpStatusCode.UnprocessableEntity,
              RatelimitingException => (int)HttpStatusCode.TooManyRequests,
              _ => (int)HttpStatusCode.InternalServerError,
          };

        private void LogException(NLogTrack? nlogTrack, Exception error)
        {
            if (!string.IsNullOrEmpty(error.Message))
                _logger.Error(nlogTrack?.GetLogMessage($"Error Message : {error.Message}"));
            if (!string.IsNullOrEmpty(error.StackTrace))
                _logger.Error(nlogTrack?.GetLogMessage($"Error Stack Trace : {error.StackTrace}"));
        }

        private void LogErrorDetails(NLogTrack? nlogTrack, int statusCode, Exception error)
        {
            _logger.Error(nlogTrack?.GetLogMessage($"StatusCode:{statusCode} Error:{error.Message}"));
            _logger.Error(nlogTrack?.GetLogMessage($"StackTrace:{error.StackTrace}"));
        }

        private void LogInfo(NLogTrack? nlogTrack, string message)
        {
            _logger.Info(nlogTrack?.GetLogMessage(message));
        }
    }
}
