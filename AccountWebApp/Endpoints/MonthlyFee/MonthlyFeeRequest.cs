namespace AccountWebApp.Endpoints.MonthlyFee;

public record class MonthlyFeeRequest
{
    public int AccountId { get; init; }
}
