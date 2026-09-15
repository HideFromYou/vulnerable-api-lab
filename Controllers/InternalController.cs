using Microsoft.AspNetCore.Mvc;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InternalController : ControllerBase
{
    [HttpGet("secret")]
    public IActionResult Secret()
    {
        return Ok(new
        {
            service = "Internal Admin Service",
            secret = "INTERNAL-SECRET-12345"
        });
    }
}