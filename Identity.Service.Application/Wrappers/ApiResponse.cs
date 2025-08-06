namespace Identity.Service.Application.Wrappers
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T PayLoad { get; set; }
        public bool Success { get; set; }
        public ApiResponse(T data, string message = "")
        {
            this.Message = message;
            this.PayLoad = data;
        }

        public ApiResponse(bool success, T data, string message = "")
        {
            this.Message = message;
            this.PayLoad = data;
            this.Success = success;
        }
    }
}