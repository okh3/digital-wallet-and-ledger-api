using System;

namespace DigitalWalletAndLedgerAPI.Models;

public class Wallet
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Balance { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Wallet() { }

    public Wallet(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Balance = 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (Balance < amount) throw new InsufficientFundsException("Insufficient funds."); 
        Balance -= amount;
        UpdatedAt = DateTime.UtcNow;
    }
}