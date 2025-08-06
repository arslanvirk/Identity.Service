namespace Identity.Service.Application.Models
{
    public class SecretManagerKeys
    {
        public required string DB_HOST { get; set; }
        public required string DB_NAME_IDENTITY { get; set; }
        public required string DB_USER { get; set; }
        public required string DB_PASSWORD { get; set; }
        public required string JWT_SECRET { get; set; }
    }
}