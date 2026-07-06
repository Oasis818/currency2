namespace FxBasic.DTOs;

public class CreateWalletRequest
{
    public int UserId { get; set; }
    public string Currency { get; set; } = "GBP";
}
