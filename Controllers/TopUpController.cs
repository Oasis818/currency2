using FxBasic.Data;
using FxBasic.DTOs;
using FxBasic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TopUpController : ControllerBase
{
    private readonly AppDbContext _db;
    public TopUpController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TopUpRequest req)
    {
        if (req.Amount <= 0) return BadRequest("Amount must be greater than 0.");

        var bank = await _db.BankAccounts.FirstOrDefaultAsync(x => x.BankAccountId == req.BankAccountId && x.UserId == req.UserId);
        if (bank is null) return NotFound("Bank account not found.");

        var wallet = await _db.WalletAccounts.FirstOrDefaultAsync(x => x.WalletAccountId == req.DestinationWalletId && x.UserId == req.UserId);
        if (wallet is null) return NotFound("Destination wallet not found.");

        // RULE: top-up from UK designated bank account into GBP wallet only
        if (!bank.IsUserMainUkAccount || bank.CountryCode != "GB")
            return BadRequest("Top-up must come from your designated UK bank account.");

        if (wallet.Currency != "GBP")
            return BadRequest("Bank top-up is allowed only into a GBP wallet.");

        wallet.Balance += req.Amount;

        _db.Transfers.Add(new Transfer
        {
            UserId = req.UserId,
            Type = TransferType.BankTopUpGbp,
            SourceCurrency = "GBP",
            DestinationCurrency = "GBP",
            SourceAmount = req.Amount,
            DestinationAmount = req.Amount,
            RateUsed = 1m,
            Status = "COMPLETED"
        });

        await _db.SaveChangesAsync();
        return Ok(new { message = "Top-up successful", wallet.Balance });
    }
}
