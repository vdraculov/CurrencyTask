namespace CurrencyConverter.Models;

public class CurrencyRateRequest
{
    public decimal Amount { get; set; }
    public string[] Currencies { get; set; } = [];
}