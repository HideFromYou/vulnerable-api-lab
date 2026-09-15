using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CsrfController : ControllerBase
{
    private readonly IAntiforgery _antiforgery;

    public CsrfController(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail([FromForm] string email)
    {
        if (!Request.Cookies.ContainsKey("csrf_session"))
        {
            return Unauthorized();
        }

        await _antiforgery.ValidateRequestAsync(HttpContext);

        return Ok(new
        {
            message = $"Email changed to {email}"
        });
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        Response.Cookies.Append("csrf_session", "authenticated-user");

        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new
        {
            message = "Logged in",
            csrfToken = tokens.RequestToken
        });
    }
}