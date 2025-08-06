#nullable enable

using System.Collections.Generic;

namespace Identity.Service.Application.Wrappers
{
    public class ErrorResponse<T>
    {
        public ErrorResponse()
        {
            Success = false;
            Message = string.Empty;
            Errors = new List<ErrorModel>();
        }

        public ErrorResponse(string message)
        {
            Success = false;
            Message = message ?? string.Empty;
            Errors = new List<ErrorModel>();
        }

        public ErrorResponse(T data, string? message = null)
        {
            Success = true;
            Message = message ?? string.Empty;
            Data = data;
            Errors = new List<ErrorModel>();
        }

        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ErrorModel> Errors { get; set; }
        public T? Data { get; set; }
    }

    public class ErrorModel
    {
        public ErrorModel()
        {

        }
        public string PropertyName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
