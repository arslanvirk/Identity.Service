using System;

namespace Identity.Service.Application.Models
{
    public class EmailSettings
    {
        public int Port { get; set; }
        public bool UseSsl { get; set; }
        public required string Host { get; set; }
        public required string Password { get; set; }
        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
    }

}
