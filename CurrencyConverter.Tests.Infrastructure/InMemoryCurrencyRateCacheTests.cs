using CurrencyConverter.Infrastructure.Cache;

namespace CurrencyConverter.Tests.Infrastructure;

public class InMemoryCurrencyRateCacheTests
{
    [Fact]
    public void Should_Return_Stored_Rates()
    {
        var cache = new InMemoryCurrencyRateCache(TimeSpan.FromMinutes(1));
        var date = DateTime.Today;
        var currencies = new[] { "USD", "CAD" };
        var expectedRates = new Dictionary<string, decimal> { { "USD", 1.1m }, { "CAD", 1.5m } };

        cache.Set(date, currencies, expectedRates);
        var success = cache.TryGet(date, currencies, out var actualRates);

        Assert.True(success);
        Assert.Equal(expectedRates, actualRates);
    }

    [Fact]
    public async Task Should_Expire_After_TTL()
    {
        var cache = new InMemoryCurrencyRateCache(TimeSpan.FromMilliseconds(100));
        var date = DateTime.Today;
        var currencies = new[] { "USD" };
        cache.Set(date, currencies, new Dictionary<string, decimal> { { "USD", 1.1m } });

        await Task.Delay(150);

        var found = cache.TryGet(date, currencies, out _);
        Assert.False(found);
    }

    [Fact]
    public void Should_Ignore_Currency_Case()
    {
        var cache = new InMemoryCurrencyRateCache();
        var date = DateTime.Today;
        cache.Set(date, new[] { "usd", "cad" }, new Dictionary<string, decimal> { { "USD", 1.1m }, { "CAD", 1.2m } });

        var found = cache.TryGet(date, new[] { "CAD", "USD" }, out var result);

        Assert.True(found);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Should_Ignore_Currency_Order()
    {
        var cache = new InMemoryCurrencyRateCache();
        var date = DateTime.Today;
        cache.Set(date, new[] { "USD", "CAD" }, new Dictionary<string, decimal> { { "USD", 1.1m }, { "CAD", 1.2m } });

        var found = cache.TryGet(date, new[] { "CAD", "USD" }, out var result);

        Assert.True(found);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Should_Remove_Entry()
    {
        var cache = new InMemoryCurrencyRateCache();
        var date = DateTime.Today;
        var currencies = new[] { "USD", "CAD" };

        cache.Set(date, currencies, new Dictionary<string, decimal> { { "USD", 1.1m }, { "CAD", 1.2m } });
        cache.Remove(date, currencies);

        var found = cache.TryGet(date, currencies, out _);
        Assert.False(found);
    }
}
