using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vulnerable_api.Data;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClickjackingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IAntiforgery _antiforgery;

    public ClickjackingController(
        AppDbContext context,
        IAntiforgery antiforgery)
    {
        _context = context;
        _antiforgery = antiforgery;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string username)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            return Unauthorized("User not found");
        }

        Response.Cookies.Append(
            "clickjacking_session",
            user.Id.ToString(),
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            }
        );

        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new
        {
            message = "Clickjacking lab login successful",
            userId = user.Id,
            csrfToken = tokens.RequestToken
        });
    }

    [HttpGet("csrf")]
    public IActionResult GetCsrfToken()
    {
        if (!Request.Cookies.ContainsKey("clickjacking_session"))
        {
            return Unauthorized("Not authenticated");
        }

        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new
        {
            csrfToken = tokens.RequestToken
        });
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail(
        [FromForm] string email)
    {
        await _antiforgery.ValidateRequestAsync(HttpContext);

        if (!Request.Cookies.TryGetValue(
                "clickjacking_session",
                out var userIdValue))
        {
            return Unauthorized("Not authenticated");
        }

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized("Invalid session");
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        user.Email = email;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Email changed successfully",
            userId = user.Id,
            newEmail = user.Email
        });

    }
    [HttpPost("card/open")]
public IActionResult OpenCard()
{
    if (!Request.Cookies.ContainsKey("clickjacking_session"))
    {
        return Unauthorized("Not authenticated");
    }

    Response.Cookies.Append(
        "clickjacking_card_opened",
        "true",
        new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax
        }
    );

    return Ok(new
    {
        message = "Card details opened"
    });
}

[HttpPost("card/reveal")]
public IActionResult RevealCard()
{
    if (!Request.Cookies.ContainsKey("clickjacking_session"))
    {
        return Unauthorized("Not authenticated");
    }

    if (!Request.Cookies.ContainsKey("clickjacking_card_opened"))
    {
        return BadRequest("Card details are not open");
    }

    return Ok(new
    {
        cardNumber = "4111 1111 1111 1111",
        expiry = "12/30",
        cvv = "123"
    });
}
}