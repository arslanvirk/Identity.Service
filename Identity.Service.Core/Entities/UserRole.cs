using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Service.Core.Entities
{
    [Table("user_role")]
    public class UserRole : IdentityRole<Guid>
    {
        [Column("description")]
        public string? Description { get; set; }
    }
}
