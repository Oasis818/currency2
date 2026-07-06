namespace FxBasic.DTOs;

public class PayoutRequest
{
    public int UserId { get; set; }
    public int SourceWalletId { get; set; }
    public int DestinationBankAccountId { get; set; } // can be any country
    public decimal Amount { get; set; }
}
