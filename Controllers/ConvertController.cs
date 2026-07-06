using FxBasic.Data;
using FxBasic.DTOs;
using FxBasic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConvertController : ControllerBase
{
    private readonly AppDbContext _db;
    public ConvertController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConvertRequest req)
    {
        if (req.Amount <= 0) return BadRequest("Amount must be greater than 0.");

        var from = await _db.WalletAccounts.FirstOrDefaultAsync(x => x.WalletAccountId == req.FromWalletId && x.UserId == req.UserId);
        var to = await _db.WalletAccounts.FirstOrDefaultAsync(x => x.WalletAccountId == req.ToWalletId && x.UserId == req.UserId);

        if (from is null || to is null) return NotFound("Wallet not found.");
        if (from.WalletAccountId == to.WalletAccountId) return BadRequest("Source and destination wallets must be different.");
        if (from.Balance < req.Amount) return BadRequest("Insufficient funds.");

        decimal rate = 1m;
        if (from.Currency != to.Currency)
        {
            var today = DateTime.UtcNow.Date;
            var fx = await _db.ExchangeRates
                .Where(r => r.FromCurrency == from.Currency && r.ToCurrency == to.Currency && r.RateDate == today)
                .FirstOrDefaultAsync();

            if (fx is null)
                return BadRequest("Daily exchange rate not found for this currency pair.");

            rate = fx.Rate;
        }

        var converted = req.Amount * rate;

        from.Balance -= req.Amount;
        to.Balance += converted;

        _db.Transfers.Add(new Transfer
        {
            UserId = req.UserId,
            Type = TransferType.InternalExchange,
            SourceCurrency = from.Currency,
            DestinationCurrency = to.Currency,
            SourceAmount = req.Amount,
            DestinationAmount = converted,
            RateUsed = rate,
            Status = "COMPLETED"
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Conversion successful", convertedAmount = converted, rateUsed = rate });
    }
}
