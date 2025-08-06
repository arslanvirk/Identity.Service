using System;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Identity.Service.Core.Entities;
using Identity.Service.Infrastructure.DataSeeder;
using Identity.Service.Application.DTOs.Shared.Configurations;

namespace Identity.Service.Infrastructure.Context
{
    public class EFDataContext : IdentityDbContext<User, UserRole, Guid>
    {
        private readonly string _defaultSchema;

        public EFDataContext(DbContextOptions<EFDataContext> options, IOptions<SchemaOptionsDto> schemaOptions)
            : base(options)
        {
            _defaultSchema = "identity_base";
        }

        public DbSet<InviteUser> InviteUsers { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(_defaultSchema);
            base.OnModelCreating(builder);
            builder.SeedConfiguration();

            builder.Entity<UserRole>().HasData(
                new UserRole { Id =new Guid("181d84e2-afb6-499b-9972-da57e1e63777"), Name = "User", NormalizedName = "USER" },
                new UserRole { Id = new Guid("181d84e2-afb6-499b-9972-da57e1e63888"), Name = "Admin", NormalizedName = "ADMIN" }
            );

            builder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(e => e.Id).HasColumnName("user_id");
            });
        }
    }
}
