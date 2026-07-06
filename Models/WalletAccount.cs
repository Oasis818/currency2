namespace FxBasic.Models;

public class WalletAccount
{
    public int WalletAccountId { get; set; }
    public int UserId { get; set; }
    public string Currency { get; set; } = "GBP";
    public decimal Balance { get; set; } = 0m;
}
