using Microsoft.AspNetCore.Mvc;
using Identity.Service.Application.Wrappers;

namespace Identity.Service.Web.Modules
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DefaultApiConventions), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<>), StatusCodes.Status400BadRequest)]
    public class BaseController : ControllerBase
    {
    }
}
