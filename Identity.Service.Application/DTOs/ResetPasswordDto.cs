using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Service.Application.DTOs
{
    public class ResetPasswordDto : NewPasswordDto
    {
        [Required]
        public required string InviteUserId { get; set; }
    }
    public class NewPasswordDto
    {
        private const int PasswordMaxLength = 50;
        private const int PasswordMinLength = 8;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "Must be between 8 characters")]
        [PasswordPropertyText(true)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "Must be between 8 characters")]
        [PasswordPropertyText(true)]
        [Compare("Password")]
        public required string ConfirmPassword { get; set; }
    }
}
