using System;
using AccountWebApp.Services.Account;

namespace AccountWebApp.Endpoints.MonthlyFee;

public static class MonthlyFeeEndppoints
{
    public static void MapMonthlyFeeEndpoints(this WebApplication app)
    {
        app.MapPost(
            "/accounts/monthly-fee", 
            async (MonthlyFeeRequest request, IAccountService accountService) =>
        {
            await accountService.MonthlyFeeAsync(request.AccountId);
            return Results.Ok();
        });
    }
}
