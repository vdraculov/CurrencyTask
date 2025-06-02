namespace CurrencyConverter.Models;

public class CurrencyRateRequestViewModel
{
    public decimal Amount { get; set; }
    public string[] Currencies { get; set; } = [];
}