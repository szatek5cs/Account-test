namespace AccountWebApp.Endpoints.Transfers;

public record class TransferRequest
{
    public int FromAccountId { get; init; }
    public int ToAccountId { get; init; }
    public decimal Amount { get; init; }
}
