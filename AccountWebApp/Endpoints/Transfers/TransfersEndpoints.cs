using System;
using AccountWebApp.Exceptions;
using AccountWebApp.Services.Transfers;

namespace AccountWebApp.Endpoints.Transfers;

public static class TransfersEndpoints
{
    public static void MapTransfersEndpoints(this WebApplication app)
    {
        app.MapPost("/transfers/batch", async (
            BatchTransfersRequest request,
            ITransfersService transfersService) =>
        {
            if (request.Transfers == null || request.Transfers.Count == 0)
            {
                throw new BadRequestException("No transfers provided.");
            }

            if (request.Transfers.Any(t => t.Amount <= 0))
            {
                throw new BadRequestException("All transfer amounts must be greater than zero.");
            }

            await transfersService.ProcessBatchTransfersAsync(request.Transfers);
            
            return Results.Ok($"Batch transfer of {request.Transfers.Count} transfers completed successfully.");
        });
    }
}
