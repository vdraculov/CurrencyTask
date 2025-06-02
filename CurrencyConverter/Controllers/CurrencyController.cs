using System.Diagnostics;
using CurrencyConverter.Application;
using Microsoft.AspNetCore.Mvc;
using CurrencyConverter.Models;

namespace CurrencyConverter.Controllers;

public class CurrencyController(Lazy<ICurrencyService> currencyService) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessRates([FromBody] CurrencyRateRequestViewModel model)
    {
        try
        {
            if (model == null || model.Currencies == null || model.Currencies.Length < 1)
                return BadRequest("Invalid input.");
            var results = await currencyService.Value.GetHistoricalRatesAsync(model.Amount, model.Currencies);
            return Json(results);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}