using CurrencyConverter.Core.Domain;
using CurrencyConverter.Infrastructure.Cache;
using CurrencyConverter.Infrastructure.Clients;

namespace CurrencyConverter.Application;

public class CurrencyService(IFrankfurterClient client, ICurrencyRateCache cache) : ICurrencyService
{
    public async Task<List<CurrencyRateResult>> GetHistoricalRatesAsync(decimal amount, string[] currencies)
    {
        if (amount == 999)
            throw new ArgumentException("Amount 999 is not allowed.");

        var results = new List<CurrencyRateResult>();

        for (int i = 0; i < 7; i++)
        {
            var monday = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - 1) - i * 7);
            var normalizedCurrencies = currencies.Select(c => c.ToUpperInvariant()).OrderBy(c => c).ToArray();

            if (!cache.TryGet(monday, normalizedCurrencies, out var rates))
            {
                rates = await client.GetRatesForDateAsync(monday, normalizedCurrencies);
                cache.Set(monday, normalizedCurrencies, rates);
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