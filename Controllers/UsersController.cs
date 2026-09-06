using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vulnerable_api.Data;
using vulnerable_api.Models;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;

    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult GetUsers()
    {
        var users = _db.Users.ToList();

        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _db.Users.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public IActionResult CreateUser(CreateUserRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Role = request.Role
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        return Ok(user);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, CreateUserRequest request)
    {
        var user = _db.Users.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        user.Username = request.Username;
        user.Email = request.Email;
        user.Role = request.Role;

        _db.SaveChanges();

        return Ok(user);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _db.Users.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        _db.Users.Remove(user);
        _db.SaveChanges();

        return NoContent();
    }
}