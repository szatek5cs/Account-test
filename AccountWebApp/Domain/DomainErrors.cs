using System;

namespace AccountWebApp.Domain;

public static class DomainErrors
{
    public const string InvalidAmount = "INVALID_AMOUNT";
    public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
    public const string MonthlyFeeAlreadyApplied = "MONTHLY_FEE_ALREADY_APPLIED";
}
