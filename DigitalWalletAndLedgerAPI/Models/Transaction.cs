using System;

namespace DigitalWalletAndLedgerAPI.Models;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid FromWalletId { get; private set; }
    public Guid ToWalletId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Timestamp { get; private set; }

    protected Transaction() { }

    public Transaction(Guid fromWalletId, Guid toWalletId, decimal amount)
    {
        Id = Guid.NewGuid();
        FromWalletId = fromWalletId;
        ToWalletId = toWalletId;
        Amount = amount;
        Timestamp = DateTime.UtcNow;
    }
}