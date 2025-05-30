using CurrencyConverter.Application;
using CurrencyConverter.Infrastructure.Cache;
using CurrencyConverter.Infrastructure.Clients;
using CurrencyConverter.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IFrankfurterClient, FrankfurterClient>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<Lazy<ICurrencyService>>(sp => 
    new Lazy<ICurrencyService>(() => {
        using var scope = sp.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ICurrencyService>();
    }));


builder.Services.AddSingleton<ICurrencyRateCache>(
    new InMemoryCurrencyRateCache(TimeSpan.FromMinutes(15)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Currency}/{action=Index}/{id?}");

app.Run();