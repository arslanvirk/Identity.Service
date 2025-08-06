using System.ComponentModel.DataAnnotations;

namespace Identity.Service.Application.DTOs;

public class RegisterDto
{
    [Required, EmailAddress]
    public required string Email { get; set; }
    [Required, MinLength(8)]
    public required string Password { get; set; }
    [Required]
    public required string FirstName { get; set; }
    [Required]
    public required string LastName { get; set; }
}
