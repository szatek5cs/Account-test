namespace AccountWebApp.Endpoints.Withdraw;

public record WithdrawRequest
{
    public int AccountId { get; init; }
    public decimal Amount { get; init; }
}
