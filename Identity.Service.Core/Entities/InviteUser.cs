using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Identity.Service.Core.Entities
{
    [Table("invite_user")]
    public class InviteUser : BaseEntity
    {
        [Key]
        [Column("invite_user_id")]
        public Guid InviteUserId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        public DateTime ExpireTime { get; set; }
        public required string Type { get; set; }
        public required string Status { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
