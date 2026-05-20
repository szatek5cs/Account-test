namespace AccountWebApp.Endpoints.Transfers;

public record class BatchTransfersRequest
{
    public List<TransferRequest> Transfers { get; init; } = new();
}
