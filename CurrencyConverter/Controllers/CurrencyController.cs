using System.Diagnostics;
using CurrencyConverter.Application;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Models;

namespace CurrencyConverter.Controllers;

public class CurrencyController : Controller
{
    private readonly ILogger<CurrencyController> _logger;

    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetRates(decimal amount, string currency1, string currency2, string currency3)
    {
        try
        {
            var results = await _currencyService.GetHistoricalRatesAsync(amount, new[] { currency1, currency2, currency3 });
            return Json(results);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}