using System;

namespace AccountWebApp.Domain;

public class Transaction
{
    public int Id { get; private set; }
    public int AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string Description { get; private set; } = default!;
    public TransactionType Type { get; private set; }

    private Transaction() { } // For EF Core

    public Transaction(
        int accountId, 
        decimal amount, 
        string description, 
        TransactionType type)
    {
        AccountId = accountId;
        Amount = amount;
        CreatedAtUtc = DateTime.UtcNow;
        Description = description;
        Type = type;
    }
}
