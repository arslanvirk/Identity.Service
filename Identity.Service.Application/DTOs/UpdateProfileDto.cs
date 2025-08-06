using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.Service.Application.DTOs;

public class UpdateProfileDto
{
    private const int PasswordMaxLength = 50;
    private const int PasswordMinLength = 8;
    [EmailAddress]
    public string? Email { get; set; }
    [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "Must be between 8 characters")]
    [PasswordPropertyText(true)]
    public string? Password { get; set; }

    [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength, ErrorMessage = "Must be between 8 characters")]
    [PasswordPropertyText(true)]
    [Compare("Password")]
    public string? ConfirmPassword { get; set; }
}
