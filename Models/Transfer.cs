namespace FxBasic.Models;

public enum TransferType
{
    InternalExchange = 1,
    BankTopUpGbp = 2,
    BankPayout = 3
}

public class Transfer
{
    public int TransferId { get; set; }
    public int UserId { get; set; }

    public TransferType Type { get; set; }

    public string SourceCurrency { get; set; } = string.Empty;
    public string DestinationCurrency { get; set; } = string.Empty;

    public decimal SourceAmount { get; set; }
    public decimal DestinationAmount { get; set; }
    public decimal RateUsed { get; set; }

    public string Status { get; set; } = "COMPLETED";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
