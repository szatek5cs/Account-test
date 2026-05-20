using System;

namespace AccountWebApp.Services.Account;

public interface IAccountService
{
    Task MonthlyFeeAsync(int accountId);
    Task WithdrawAsync(int accountId, decimal amount);
}
