using System;
using AccountWebApp.Endpoints.Transfers;

namespace AccountWebApp.Services.Transfers;

public interface ITransfersService
{
    Task ProcessBatchTransfersAsync(List<TransferRequest> transfers);
}
