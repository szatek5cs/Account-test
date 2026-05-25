using System;
using System.ComponentModel.DataAnnotations;
using AccountWebApp.Exceptions;

namespace AccountWebApp.Domain;

public class Account
{
    public int Id { get; private set; }
    public decimal Balance { get; private set; }

    private Account() { } // For EF Core

    public Account(decimal initialBalance = 0)
    {
        Balance = initialBalance;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainValidationException("Amount must be greater than zero.", DomainErrors.InvalidAmount);
        }

        if (Balance < amount)
        {
            throw new DomainValidationException("Insufficient funds for withdrawal.", DomainErrors.InsufficientFunds);
        }

        Balance -= amount;
    }

    public decimal MonthlyFee(List<Transaction> thisMonthTransactions)
    {
        if (thisMonthTransactions.Any(t => t.Type == TransactionType.MonthlyFee))
        {
            throw new DomainValidationException("Monthly fee has already been applied for this month.", DomainErrors.MonthlyFeeAlreadyApplied);
        }

        Balance -= DomainConsts.MonthlyFeeAmount;

        return DomainConsts.MonthlyFeeAmount;
    }

    internal void TransferDebit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainValidationException("Amount must be greater than zero.", DomainErrors.InvalidAmount);
        }

        if (Balance < amount)
        {
            throw new DomainValidationException("Insufficient funds for transfer.", DomainErrors.InsufficientFunds);
        }

        Balance -= amount;
    }

    internal void TransferCredit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainValidationException("Amount must be greater than zero.", DomainErrors.InvalidAmount);
        }

        Balance += amount;
    }
}
