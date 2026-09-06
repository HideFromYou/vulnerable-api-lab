using Microsoft.AspNetCore.Mvc;
using vulnerable_api.Data;
using vulnerable_api.Models;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = request.Password,
            Role = "user"
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _db.Users
            .FirstOrDefault(u =>
                u.Username == request.Username &&
                u.Password == request.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }
}