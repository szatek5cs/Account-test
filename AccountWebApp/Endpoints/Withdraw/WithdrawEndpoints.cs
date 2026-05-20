using System;
using AccountWebApp.Exceptions;
using AccountWebApp.Services.Account;

namespace AccountWebApp.Endpoints.Withdraw;

public static class WithdrawEndpoints
{
    public static void MapWithdrawEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/accounts/withdraw", 
            async (WithdrawRequest request, IAccountService accountService) =>
        {
            if (request.Amount <= 0)
            {
                throw new BadRequestException("Amount must be greater than zero.");
            }
            
            await accountService.WithdrawAsync(request.AccountId, request.Amount);
            return Results.Ok();
        });
    }
}
