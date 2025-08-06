namespace Identity.Service.Application.Constants
{
    public static class ConfigurationKey
    {
        public readonly static string TokenExpiryTimeInMinutes = "TokenExpiryTimeInMinutes";
        public readonly static string TokenExpiryValue = "1440";
        public readonly static string LinkExpiryTimeInMinutes = "LinkExpiryTimeInMinutes";
        public readonly static string LinkExpiryValue = "1440";
        public readonly static string UserDeactivationTime = "UserDeactivationTime";
        public readonly static string UserDeactivationTimeValue = "40320";
        public readonly static string UserDeletionTime = "UserDeletionTime";
        public readonly static string UserDeletionTimeValue = "3153600";
        public readonly static string InvoiceDeletionTime = "InvoiceDeletionTime";
        public readonly static string InvoiceDeletionValue = "5256000";
        public readonly static string ApiTokenExpiryValue = "5256000";
        public readonly static int RequiredLength = 8;
        public readonly static int MaxFailedAccessAttempts = 5;
    }
}
