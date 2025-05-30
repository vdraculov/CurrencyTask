namespace CurrencyConverter.Core.Domain;

public class CurrencyRateResult
{
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new();
    public Dictionary<string, decimal> Converted { get; set; } = new();
}