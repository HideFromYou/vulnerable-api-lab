using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet("{id:int}")]
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

    [HttpGet("search")]
    public IActionResult Search(string username)
    {
        var sql = $"SELECT * FROM Users WHERE Username = '{username}'";

var users = _db.Users
    .FromSqlRaw(sql)
    .ToList();

        return Ok(users);
    }

    [HttpGet("reflect")]
    public IActionResult Reflect(string input)
    {
        return Content($"You searched for: {input}", "text/html");
    }

    [HttpGet("js")]
    public IActionResult Js(string name)
    {
        return Content(
            $"<script>let username = \"{name}\";</script>",
            "text/html"
        );
    }

    [HttpGet("file")]
    public IActionResult GetFile(string name)
    {
        var path = Path.Combine("uploads", name);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        var content = System.IO.File.ReadAllText(path);

        return Ok(content);
    }

    [HttpGet("ping")]
public IActionResult Ping(string host)
{
    var command = $"ping -c 1 {host}";
    var result = System.Diagnostics.Process.Start(
        new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/bin/sh",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });

    var output = result?.StandardOutput.ReadToEnd();

    return Ok(output);
}
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    Directory.CreateDirectory(uploadsPath);

    var filePath = Path.Combine(uploadsPath, file.FileName);

    using var stream = new FileStream(filePath, FileMode.Create);

    await file.CopyToAsync(stream);

    return Ok(new
    {
        fileName = file.FileName,
        path = filePath
    });
}

}