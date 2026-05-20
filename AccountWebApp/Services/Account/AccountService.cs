using System;
using AccountWebApp.Domain;
using AccountWebApp.Exceptions;
using AccountWebApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccountWebApp.Services.Account;

public class AccountService(AppDbContext dbContext) : IAccountService
{
    public async Task MonthlyFeeAsync(int accountId)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new NotFoundException(nameof(Account), accountId);
        }

        var thisMonthTransactions = await dbContext.Transactions
            .Where(t => t.AccountId == accountId 
                && t.CreatedAtUtc.Year == DateTime.UtcNow.Year
                && t.CreatedAtUtc.Month == DateTime.UtcNow.Month)
            .ToListAsync();

        var amount = account.MonthlyFee(thisMonthTransactions);

        var transaction = new Transaction(accountId, amount, "Monthly Fee", TransactionType.MonthlyFee);
        dbContext.Transactions.Add(transaction);

        await dbContext.SaveChangesAsync();
    }

    public async Task WithdrawAsync(int accountId, decimal amount)
    {
        var account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account == null)
        {
            throw new NotFoundException(nameof(Account), accountId);
        }

        if (account.Balance < amount)
        {
            throw new BadRequestException("Insufficient funds.");
        }

        account.Withdraw(amount);

        var transaction = new Transaction(accountId, amount, "Withdrawal", TransactionType.Withdraw);
        dbContext.Transactions.Add(transaction);

        await dbContext.SaveChangesAsync();
    }
}
