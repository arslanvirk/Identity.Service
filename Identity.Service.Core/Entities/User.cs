using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Service.Core.Entities
{
    [Table("user")]
    public class User : IdentityUser<Guid>
    {
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("updated_by")]
        public required string UpdatedBy { get; set; }
        [Column("password_updated_at")]
        public DateTime? PasswordUpdatedAt { get; set; }
    }
}
