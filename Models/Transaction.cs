namespace vulnerable_api.Models;

public class Transaction
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public int RecipientId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}