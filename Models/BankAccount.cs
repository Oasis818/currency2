namespace FxBasic.Models;

public class BankAccount
{
    public int BankAccountId { get; set; }
    public int UserId { get; set; }

    public string AccountName { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;

    // For UK accounts
    public string? SortCode { get; set; }
    public string? AccountNumber { get; set; }

    // For international accounts
    public string? Iban { get; set; }
    public string? SwiftBic { get; set; }

    public string CountryCode { get; set; } = "GB";   // e.g. GB, FR, US
    public string Currency { get; set; } = "GBP";     // account currency
    public bool IsUserMainUkAccount { get; set; } = false;
}
