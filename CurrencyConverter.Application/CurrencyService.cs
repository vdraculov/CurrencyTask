using CurrencyConverter.Core.Domain;
using CurrencyConverter.Infrastructure.Clients;

namespace CurrencyConverter.Application;

public class CurrencyService(IFrankfurterClient client) : ICurrencyService
{
    private readonly Dictionary<string, Dictionary<string, decimal>> _cache = new();

    public async Task<List<CurrencyRateResult>> GetHistoricalRatesAsync(decimal amount, string[] currencies)
    {
        if (amount == 999)
            throw new ArgumentException("Amount 999 is not allowed.");

        var results = new List<CurrencyRateResult>();

        for (int i = 0; i < 7; i++)
        {
            var monday = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - 1) - i * 7);
            var normalizedCurrencies = currencies.Select(c => c.ToUpperInvariant()).OrderBy(c => c).ToArray();
            var currencyKey = string.Join("-", normalizedCurrencies);
            var cacheKey = $"{monday:yyyy-MM-dd}_{currencyKey}";

            if (!_cache.TryGetValue(cacheKey, out var rates))
            {
                rates = await client.GetRatesForDateAsync(monday, normalizedCurrencies);
                _cache[cacheKey] = rates;
            }

            results.Add(new CurrencyRateResult
            {
                Date = monday,
                Rates = rates,
                Converted = rates.ToDictionary(kv => kv.Key, kv => kv.Value * amount)
            });
        }

        return results.OrderBy(r => r.Date).ToList();
    }
}