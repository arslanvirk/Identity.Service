using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Identity.Service.Web.Helpers;
using Identity.Service.Application.DTOs;
using Identity.Service.Application.Helpers;
using Identity.Service.Application.Wrappers;
using Identity.Service.Application.Constants;
using Identity.Service.Application.IServices;
using static Identity.Service.Application.Constants.Messages;

namespace Identity.Service.Web.Modules.Users.Controllers;

public class UserController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IUserProfileService _profileService;
    private readonly DataProtector _protector;
    private readonly AuthHelper _authHelper;

    public UserController(
        IAuthService authService,
        IUserProfileService profileService,
        DataProtector protector,
        AuthHelper authHelper)
    {
        _authService = authService;
        _profileService = profileService;
        _protector = protector;
        _authHelper = authHelper;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        return Ok(await _authService.RegisterAsync(dto));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseModel>>> Login([FromBody] LoginDto dto)
    {
        var response = await _authService.LoginAsync(dto);

        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = response.Expires,
            Path = "/"
        };

        Response.Cookies.Append(
          AuthCookiesValue.AuthKey,
          _protector.Encrypt(response?.Token ?? string.Empty),
          options);

        LoginResponseModel userProfile = new LoginResponseModel
        {
            Token = response?.Token ?? string.Empty
        };
        return Ok(new ApiResponse<LoginResponseModel>(true, userProfile, LoginSuccess));
    }

    [HttpGet("profile")]
    [AuthorizeUserAttribute]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        return Ok(await _profileService.GetAsync(HttpContext.GetUserId()));
    }

    [HttpPut("profile")]
    [AuthorizeUserAttribute]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        return Ok(await _profileService.UpdateAsync(HttpContext.GetUserId(), dto));
    }
    /// <summary>
    /// forget password request
    /// </summary> 
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        return Ok(await _authService.ForgotPasswordAsync(dto));
    }

    /// <summary>
    /// update password
    /// </summary> 
    [HttpPost("change-password")]
    [AuthorizeUserAttribute]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        return Ok(await _authService.ChangePasswordAsync(HttpContext.GetUserId(), dto));
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        return Ok(await _authService.ResetPasswordAsync(dto));
    }
    /// <summary>
    /// logout
    /// </summary> 
    [HttpGet("logout")]
    [AuthorizeUserAttribute]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOut();
        _authHelper.ExpireCookies();
        return Ok(new ApiResponse<dynamic>(true, string.Empty, LogoutSuccess));
    }
}
