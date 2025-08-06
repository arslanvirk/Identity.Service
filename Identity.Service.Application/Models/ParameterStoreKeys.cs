namespace Identity.Service.Application.Models
{
    public class ParameterStoreKeys
    {
        public required string JwtValidIssuer { get; set; }
        public required string JwtValidAudience { get; set; }
        public required string LogGroup_Identity { get; set; }
    }
}