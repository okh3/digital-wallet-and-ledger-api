using DigitalWalletAndLedgerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalWalletAndLedgerAPI.Services.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Timestamp);

        modelBuilder.Entity<Wallet>()
            .Property(w => w.Balance)
            .HasColumnType("decimal(18, 4)")
            .IsConcurrencyToken(); 

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasColumnType("decimal(18, 4)");
    }
}