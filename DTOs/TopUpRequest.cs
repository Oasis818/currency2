namespace FxBasic.DTOs;

public class TopUpRequest
{
    public int UserId { get; set; }
    public int BankAccountId { get; set; }          // must be UK designated
    public int DestinationWalletId { get; set; }    // must be GBP wallet
    public decimal Amount { get; set; }
}
