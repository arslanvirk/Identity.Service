using System.ComponentModel.DataAnnotations;

namespace Identity.Service.Application.DTOs;

public class UserProfileDto
{
    public Guid Id { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }

    public UserProfileDto(Guid id, string email, string firstName, string lastName)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }
}
