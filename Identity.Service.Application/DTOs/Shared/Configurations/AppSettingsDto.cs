using System;

namespace Identity.Service.Application.DTOs.Shared.Configurations
{
    public class AppSettingsDto
    {
        public required SchemaOptionsDto Schema { get; set; }
        public static string? EnvironmentName { get; set; }
        public static string? ConnectionString { get; set; }
    }
}
