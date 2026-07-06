using FxBasic.Data;
using FxBasic.DTOs;
using FxBasic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FxBasic.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController : ControllerBase
{
    private readonly AppDbContext _db;
    public WalletsController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> CreateWallet([FromBody] CreateWalletRequest req)
    {
        var currency = req.Currency.Trim().ToUpperInvariant();
        if (currency.Length != 3) return BadRequest("Currency must be 3-letter code.");

        var exists = await _db.WalletAccounts.AnyAsync(x => x.UserId == req.UserId && x.Currency == currency);
        if (exists) return BadRequest("Wallet for this currency already exists.");

        var wallet = new WalletAccount
        {
            UserId = req.UserId,
            Currency = currency,
            Balance = 0m
        };

        _db.WalletAccounts.Add(wallet);
        await _db.SaveChangesAsync();
        return Ok(wallet);
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetWallets(int userId)
    {
        var wallets = await _db.WalletAccounts.Where(x => x.UserId == userId).ToListAsync();
        return Ok(wallets);
    }
}
