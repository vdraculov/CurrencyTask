using System.Net.Http.Json;

namespace CurrencyConverter.Infrastructure.Clients;

public class FrankfurterClient: IFrankfurterClient
{
    private readonly HttpClient _http;

    public FrankfurterClient(HttpClient httpClient)
    {
        _http = httpClient;
    }

    public async Task<Dictionary<string, decimal>> GetRatesForDateAsync(DateTime date, string[] currencies)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        var joined = string.Join(",", currencies);
        var url = $"https://api.frankfurter.app/{dateStr}?from=EUR&to={joined}";

        var result = await _http.GetFromJsonAsync<FrankfurterResponse>(url)
                     ?? throw new Exception("Invalid response from Frankfurter API");

        return result.Rates;
    }

    private class FrankfurterResponse
    {
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}

public interface IFrankfurterClient
{
    Task<Dictionary<string, decimal>> GetRatesForDateAsync(DateTime date, string[] currencies);
}