using FxBasic.Models;
using Microsoft.EntityFrameworkCore;

namespace FxBasic.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<WalletAccount> WalletAccounts => Set<WalletAccount>();
    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<ExchangeRate> ExchangeRates => Set<ExchangeRate>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WalletAccount>()
            .HasIndex(x => new { x.UserId, x.Currency })
            .IsUnique();

        modelBuilder.Entity<ExchangeRate>()
            .HasIndex(x => new { x.FromCurrency, x.ToCurrency, x.RateDate })
            .IsUnique();
    }
}
