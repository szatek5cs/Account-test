using System;
using AccountWebApp.Domain;
using AccountWebApp.Endpoints.Transfers;
using AccountWebApp.Exceptions;
using AccountWebApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AccountWebApp.Services.Transfers;

public class TransfersService(AppDbContext appDbContext) : ITransfersService
{
    public async Task ProcessBatchTransfersAsync(List<TransferRequest> transfers)
    {
        var accountsIds = transfers.SelectMany(t => new[] { t.FromAccountId, t.ToAccountId }).Distinct().ToList();
        var accountsDict = await appDbContext.Accounts.Where(a => accountsIds.Contains(a.Id)).ToDictionaryAsync(a => a.Id, a => a);

        if (accountsDict.Count != accountsIds.Count)
        {
            throw new NotFoundException(nameof(Account), accountsIds.Except(accountsDict.Keys).First());
        }

        foreach(var transfer in transfers)
        {
            var accountFrom = accountsDict[transfer.FromAccountId]  ;
            var accountTo = accountsDict[transfer.ToAccountId];

            accountFrom.TransferDebit(transfer.Amount);
            accountTo.TransferCredit(transfer.Amount);

            var transactionFrom = new Transaction(transfer.FromAccountId, transfer.Amount, $"Transfer to account {transfer.ToAccountId}", TransactionType.Transfer);
            var transactionTo = new Transaction(transfer.ToAccountId, transfer.Amount, $"Transfer from account {transfer.FromAccountId}", TransactionType.Transfer);

            appDbContext.Transactions.Add(transactionFrom);
            appDbContext.Transactions.Add(transactionTo);
        }

        await appDbContext.SaveChangesAsync();
    }
}
