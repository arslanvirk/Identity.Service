namespace Identity.Service.Application.Constants
{
    public static class EnvironmentNames
    {
        public static readonly string Development = "dev";
        public static readonly string Staging = "staging";
        public static readonly string Production = "prod";
        public static readonly string Testing = "Testing";
        public static string? DatabaseName { get; set; }
    }
}
