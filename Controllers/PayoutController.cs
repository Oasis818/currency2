using FxBasic.Data;
using FxBasic.DTOs;
using FxBasic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayoutController : ControllerBase
{
    private readonly AppDbContext _db;
    public PayoutController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PayoutRequest req)
    {
        if (req.Amount <= 0) return BadRequest("Amount must be greater than 0.");

        var wallet = await _db.WalletAccounts.FirstOrDefaultAsync(x => x.WalletAccountId == req.SourceWalletId && x.UserId == req.UserId);
        if (wallet is null) return NotFound("Source wallet not found.");

        var bank = await _db.BankAccounts.FirstOrDefaultAsync(x => x.BankAccountId == req.DestinationBankAccountId);
        if (bank is null) return NotFound("Destination bank account not found.");

        if (wallet.Balance < req.Amount) return BadRequest("Insufficient funds.");

        // RULE: destination account currency must match source wallet currency
        if (!string.Equals(wallet.Currency, bank.Currency, StringComparison.OrdinalIgnoreCase))
            return BadRequest("Destination bank account currency differs from source wallet. Convert first, then payout.");

        wallet.Balance -= req.Amount;

        _db.Transfers.Add(new Transfer
        {
            UserId = req.UserId,
            Type = TransferType.BankPayout,
            SourceCurrency = wallet.Currency,
            DestinationCurrency = bank.Currency,
            SourceAmount = req.Amount,
            DestinationAmount = req.Amount,
            RateUsed = 1m,
            Status = "COMPLETED"
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Payout successful", remainingBalance = wallet.Balance });
    }
}
