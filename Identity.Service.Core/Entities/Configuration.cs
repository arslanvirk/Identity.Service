using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Identity.Service.Core.Entities;

namespace Identity.Service.Core.Entities
{
    [Table("configuration")]
    public class Configuration : BaseEntity
    {
        [Key]
        [Column("configuration_id")]
        public Guid ConfigurationId { get; set; }
        [Column("configuration_key")]
        public required string ConfigurationKey { get; set; }
        [Column("configuration_value")]
        public required string ConfigurationValue { get; set; }
    }
}