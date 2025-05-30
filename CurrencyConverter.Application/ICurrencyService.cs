using CurrencyConverter.Core.Domain;

namespace CurrencyConverter.Application;

public interface ICurrencyService
{
    Task<List<CurrencyRateResult>> GetHistoricalRatesAsync(decimal amount, string[] strings);
}