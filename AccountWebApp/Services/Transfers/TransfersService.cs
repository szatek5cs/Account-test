using System;
using AccountWebApp.Domain;
using AccountWebApp.Endpoints.Transfers;
using AccountWebApp.Exceptions;
using AccountWebApp.Infrastructure.Persistence;

namespace AccountWebApp.Services.Transfers;

public class TransfersService(AppDbContext appDbContext) : ITransfersService
{
    public async Task ProcessBatchTransfersAsync(List<TransferRequest> transfers)
    {
        foreach(var transfer in transfers)
        {
            var accountFrom = appDbContext.Accounts.FirstOrDefault(a => a.Id == transfer.FromAccountId);
            if (accountFrom == null)
            {
                throw new NotFoundException(nameof(Account), transfer.FromAccountId);
            }
            
            var accountTo = appDbContext.Accounts.FirstOrDefault(a => a.Id == transfer.ToAccountId);

            if (accountTo == null)
            {
                throw new NotFoundException(nameof(Account), transfer.ToAccountId);
            }
    
            accountFrom.TransferDebit(transfer.Amount);
            accountTo.TransferCredit(transfer.Amount);

            var transactionFrom = new Transaction(transfer.FromAccountId, transfer.Amount, $"Transfer to account {transfer.ToAccountId}", TransactionType.Transfer);
            var transactionTo = new Transaction(transfer.ToAccountId, transfer.Amount, $"Transfer from account {transfer.FromAccountId}", TransactionType.Transfer);

            appDbContext.Transactions.Add(transactionFrom);
            appDbContext.Transactions.Add(transactionTo);
        }

        appDbContext.SaveChanges();
    }
}
