using Microsoft.AspNetCore.Http;

namespace Identity.Service.Application.Helpers
{
    public class AuthHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void ExpireCookies()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("HttpContext is not available.");
            }

            var cookies = _httpContextAccessor.HttpContext.Request.Cookies.Keys;
            if (cookies.Count == 0) 
            {
                return; // No cookies to expire
            }

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(-1),
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                HttpOnly = true,
            };

            foreach (var cookie in cookies)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Append(cookie, string.Empty, cookieOptions);
            }
        }
    }
}
