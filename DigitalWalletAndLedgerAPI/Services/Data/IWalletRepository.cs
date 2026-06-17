using System;
using System.Threading.Tasks;
using DigitalWalletAndLedgerAPI.Models;

namespace DigitalWalletAndLedgerAPI.Services.Data;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid walletId);
    Task AddTransactionAsync(Transaction transaction);
    Task SaveChangesAsync();
    Task AddWalletAsync(Wallet wallet);
}