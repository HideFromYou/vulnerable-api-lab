using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("super-secret-development-key-12345"));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "vulnerable-api",
            audience: "vulnerable-api",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}