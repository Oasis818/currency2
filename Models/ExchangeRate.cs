namespace FxBasic.Models;

public class ExchangeRate
{
    public int ExchangeRateId { get; set; }
    public string FromCurrency { get; set; } = "GBP";
    public string ToCurrency { get; set; } = "EUR";
    public decimal Rate { get; set; }
    public DateTime RateDate { get; set; } = DateTime.UtcNow.Date; // daily rate
}
