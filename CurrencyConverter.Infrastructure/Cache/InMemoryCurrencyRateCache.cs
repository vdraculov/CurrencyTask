using System.Collections.Concurrent;

namespace CurrencyConverter.Infrastructure.Cache;

public class InMemoryCurrencyRateCache(TimeSpan? ttl = null) : ICurrencyRateCache
{
    private readonly ConcurrentDictionary<string, (DateTime ExpiresAt, Dictionary<string, decimal> Rates)> _cache = new();
    private readonly TimeSpan _ttl = ttl ?? TimeSpan.FromMinutes(15); // default TTL

    private string GetKey(DateTime date, string[] currencies)
    {
        var key = string.Join("-", currencies.Select(c => c.ToUpperInvariant()).OrderBy(c => c));
        return $"{date:yyyy-MM-dd}_{key}";
    }

    public bool TryGet(DateTime date, string[] currencies, out Dictionary<string, decimal> rates)
    {
        var key = GetKey(date, currencies);

        if (_cache.TryGetValue(key, out var entry))
        {
            if (DateTime.UtcNow < entry.ExpiresAt)
            {
                rates = entry.Rates;
                return true;
            }
            else
            {
                _cache.TryRemove(key, out _); // Expired
            }
        }

        rates = null!;
        return false;
    }

    public void Set(DateTime date, string[] currencies, Dictionary<string, decimal> rates)
    {
        var key = GetKey(date, currencies);
        _cache[key] = (DateTime.UtcNow + _ttl, rates);
    }
}

public interface ICurrencyRateCache
{
    bool TryGet(DateTime date, string[] currencies, out Dictionary<string, decimal> rates);
    void Set(DateTime date, string[] currencies, Dictionary<string, decimal> rates);
}