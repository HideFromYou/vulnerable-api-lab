using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vulnerable_api.Data;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SqliController : ControllerBase
{
    private readonly AppDbContext _db;

    public SqliController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("basic")]
    public IActionResult Basic(string userInput)
    {
        var sql = $"""
            SELECT id, username, email, password, role
            FROM Users
            WHERE Username = '{userInput}'
            """;

        using var connection = _db.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        using var command = connection.CreateCommand();
        command.CommandText = sql;

        using var reader = command.ExecuteReader();

        var rows = new List<object[]>();

        while (reader.Read())
        {
            var row = new object[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.IsDBNull(i)
                    ? DBNull.Value
                    : reader.GetValue(i);
            }

            rows.Add(row);
        }

        return Ok(rows);
    }
[HttpGet("blind")]
public IActionResult Blind(string userInput)
{
    var sql = $"""
        SELECT 1
        FROM Users
        WHERE Username = '{userInput}'
        LIMIT 1
        """;

    using var connection = _db.Database.GetDbConnection();

    if (connection.State != System.Data.ConnectionState.Open)
    {
        connection.Open();
    }

    using var command = connection.CreateCommand();
    command.CommandText = sql;

    var result = command.ExecuteScalar();

    if (result != null)
    {
        return Ok("You've found me!");
    }

    return Ok("Nothing found.");
}
[HttpGet("error")]
public IActionResult Error(string trackingId)
{
    var sql = $"""
        SELECT 1
        FROM Users
        WHERE Username = '{trackingId}'
        LIMIT 1
        """;

    using var connection = _db.Database.GetDbConnection();

    if (connection.State != System.Data.ConnectionState.Open)
    {
        connection.Open();
    }

    using var command = connection.CreateCommand();
    command.CommandText = sql;

    command.ExecuteScalar();

    return Ok("Request processed.");
}
}

