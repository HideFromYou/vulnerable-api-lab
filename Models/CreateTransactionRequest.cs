namespace vulnerable_api.Models;

public class CreateTransactionRequest
{
    public int RecipientId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;
}