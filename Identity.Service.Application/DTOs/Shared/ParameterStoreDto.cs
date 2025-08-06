namespace Identity.Service.Application.DTOs.Shared
{
    public static class ParameterStoreDto
    {
        public static string? AwsEmailRegion { get; set; }
        public static string? JwtValidIssuer { get; set; }
        public static string? JwtValidAudience { get; set; }
        public static string? LogGroup_Rna { get; set; }
        public static string? BaseUrlIdentity_FE { get; set; }
        public static string? Email_Identity { get; set; }
    }
}
