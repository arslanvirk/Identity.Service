namespace Identity.Service.Application.DTOs;

public class AuthResponseDto : LoginResponseModel
{
    public DateTime Expires { get; set; }

    public AuthResponseDto(string token, DateTime expires)
    {
        Token = token;
        Expires = expires;
    }
}
public class LoginResponseModel
{
    public string? Token { get; set; }
}
