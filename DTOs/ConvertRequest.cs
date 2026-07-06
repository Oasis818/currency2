namespace FxBasic.DTOs;

public class ConvertRequest
{
    public int UserId { get; set; }
    public int FromWalletId { get; set; }
    public int ToWalletId { get; set; }
    public decimal Amount { get; set; }
}
