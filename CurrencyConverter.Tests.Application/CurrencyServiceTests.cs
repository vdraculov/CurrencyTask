using CurrencyConverter.Application;
using CurrencyConverter.Infrastructure.Cache;
using CurrencyConverter.Infrastructure.Clients;
using Moq;
using Xunit;

namespace CurrencyConverter.Tests.Application;

public class CurrencyServiceTests
{
    [Fact]
    public async Task GetHistoricalRatesAsync_ShouldUseDifferentCacheKeys_ForDifferentCurrencyCombinations()
    {
        // Arrange
        var date = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek - 1)); // Most recent Monday
        var callCount = 0;

        var mockClient = new Mock<IFrankfurterClient>();
        mockClient
            .Setup(x => x.GetRatesForDateAsync(It.IsAny<DateTime>(), It.IsAny<string[]>()))
            .Returns((DateTime _, string[] currencies) =>
            {
                callCount++;
                var result = currencies.ToDictionary(c => c, c => (decimal)(callCount + c.Length));
                return Task.FromResult(result);
            });

        var service = new CurrencyService(mockClient.Object, new InMemoryCurrencyRateCache(TimeSpan.FromMinutes(15)));

        // Act
        var result1 = await service.GetHistoricalRatesAsync(1, new[] { "USD", "CAD" });
        var result2 = await service.GetHistoricalRatesAsync(1, new[] { "ILS", "CAD" });

        // Assert
        Assert.NotEqual(result1[0].Converted, result2[0].Converted); // Ensure result changed
        Assert.Equal(14, callCount); // 7 dates × 2 distinct currency sets = 14 cache misses (API calls)

        for (int i = 0; i < 7; i++)
        {
            Assert.NotEqual(result1[i].Converted, result2[i].Converted);
        }
    }
}