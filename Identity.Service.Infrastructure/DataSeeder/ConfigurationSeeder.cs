using System;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Core.Entities;
using Identity.Service.Application.Constants;

namespace Identity.Service.Infrastructure.DataSeeder
{
    public static class ConfigurationSeeder
    {
        public static void SeedConfiguration(this ModelBuilder builder)
        {
            List<Configuration> configurations = new();

            #region Configuration
            configurations.Add(new Configuration
            {
                ConfigurationId = Guid.Parse("9E8AF20E-ACEE-4615-B4AA-0B4F6363002C"),
                ConfigurationKey = ConfigurationKey.TokenExpiryTimeInMinutes,
                ConfigurationValue = ConfigurationKey.TokenExpiryValue,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            });
            configurations.Add(new Configuration
            {
                ConfigurationId = Guid.Parse("181D84E2-AFB6-499B-9972-DA57E1E63578"),
                ConfigurationKey = ConfigurationKey.LinkExpiryTimeInMinutes,
                ConfigurationValue = ConfigurationKey.LinkExpiryValue,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            });

            configurations.Add(new Configuration
            {
                ConfigurationId = Guid.Parse("69FF53D7-84EB-4190-A36D-5847054E6F2D"),
                ConfigurationKey = ConfigurationKey.UserDeactivationTime,
                ConfigurationValue = ConfigurationKey.UserDeactivationTimeValue,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            });

            configurations.Add(new Configuration
            {
                ConfigurationId = Guid.Parse("E3EEB89E-764F-4570-BABA-8102A0A553ED"),
                ConfigurationKey = ConfigurationKey.UserDeletionTime,
                ConfigurationValue = ConfigurationKey.UserDeletionTimeValue,
                IsActive = true,
                CreatedAt = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            });

            #endregion

            builder.Entity<Configuration>().HasData(configurations);
        }
    }
}
