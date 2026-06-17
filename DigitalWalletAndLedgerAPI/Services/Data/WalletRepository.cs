using System;
using System.Threading.Tasks;
using DigitalWalletAndLedgerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalWalletAndLedgerAPI.Services.Data;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _context;

    public WalletRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByIdAsync(Guid walletId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.Id == walletId);
    }

    public async Task AddWalletAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}