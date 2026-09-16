using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using vulnerable_api.Data;
using vulnerable_api.Models;

namespace vulnerable_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TransactionsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult CreateTransaction(CreateTransactionRequest request)
    {
        var senderId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var recipient = _db.Users.Find(request.RecipientId);

        if (recipient == null)
        {
            return NotFound("Recipient not found");
        }

        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than zero");
        }

        var transaction = new Transaction
        {
            SenderId = senderId,
            RecipientId = request.RecipientId,
            Amount = request.Amount,
            Description = request.Description
        };

        _db.Transactions.Add(transaction);
        _db.SaveChanges();

        return Ok(transaction);
    }
    [HttpGet]
    public IActionResult GetTransactions()
{
    var userId = int.Parse(
        User.FindFirstValue(ClaimTypes.NameIdentifier)!
    );

    var transactions = _db.Transactions
        .Where(t => t.SenderId == userId || t.RecipientId == userId)
        .OrderByDescending(t => t.CreatedAt)
        .ToList();

    return Ok(transactions);
}
}
