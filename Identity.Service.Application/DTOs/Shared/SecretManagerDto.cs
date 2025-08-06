
namespace Identity.Service.Application.DTOs.Shared
{
    public static class SecretManagerDto
    {
        public static string? JwtSecret { get; set; }
        public static string? DB_HOST { get; set; }
        public static string? DB_NAME_IDENTITY { get; set; }
        public static string? DB_USER { get; set; }
        public static string? DB_PASSWORD { get; set; }
    }
}
