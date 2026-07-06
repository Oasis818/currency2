using FxBasic.Data;
using FxBasic.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=fxbasic.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Seed minimal demo data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.Add(new User { FullName = "Demo User", Email = "demo@example.com" });
        db.SaveChanges();
    }

    var user = db.Users.First();

    if (!db.BankAccounts.Any())
    {
        db.BankAccounts.AddRange(
            new BankAccount
            {
                UserId = user.UserId,
                AccountName = "Main UK Account",
                BankName = "HSBC UK",
                SortCode = "10-20-30",
                AccountNumber = "12345678",
                CountryCode = "GB",
                Currency = "GBP",
                IsUserMainUkAccount = true
            },
            new BankAccount
            {
                UserId = user.UserId,
                AccountName = "EU Beneficiary",
                BankName = "Deutsche Bank",
                Iban = "DE89370400440532013000",
                SwiftBic = "DEUTDEFF",
                CountryCode = "DE",
                Currency = "EUR",
                IsUserMainUkAccount = false
            }
        );
        db.SaveChanges();
    }

    if (!db.WalletAccounts.Any())
    {
        db.WalletAccounts.AddRange(
            new WalletAccount { UserId = user.UserId, Currency = "GBP", Balance = 1000m },
            new WalletAccount { UserId = user.UserId, Currency = "EUR", Balance = 100m }
        );
        db.SaveChanges();
    }

    var today = DateTime.UtcNow.Date;
    if (!db.ExchangeRates.Any(x => x.RateDate == today))
    {
        db.ExchangeRates.AddRange(
            new ExchangeRate { FromCurrency = "GBP", ToCurrency = "EUR", Rate = 1.17m, RateDate = today },
            new ExchangeRate { FromCurrency = "EUR", ToCurrency = "GBP", Rate = 0.85m, RateDate = today }
        );
        db.SaveChanges();
    }
}

app.Run();
