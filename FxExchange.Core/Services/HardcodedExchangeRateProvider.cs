using FxExchange.Core.Models;

namespace FxExchange.Core.Services;

public class HardcodedExchangeRateProvider : IExchangeRateProvider
{
    private readonly Dictionary<string, ExchangeRate> _rates = new(StringComparer.OrdinalIgnoreCase)
    {
        { "EUR", new ExchangeRate(new Currency("EUR", "Euro"), 743.94m) },
        { "USD", new ExchangeRate(new Currency("USD", "Amerikanske dollar"), 663.11m) },
        { "GBP", new ExchangeRate(new Currency("GBP", "Britiske pund"), 852.85m) },
        { "SEK", new ExchangeRate(new Currency("SEK", "Svenske kroner"), 76.10m) },
        { "NOK", new ExchangeRate(new Currency("NOK", "Norske kroner"), 78.40m) },
        { "CHF", new ExchangeRate(new Currency("CHF", "Schweiziske franc"), 683.58m) },
        { "JPY", new ExchangeRate(new Currency("JPY", "Japanske yen"), 5.9740m) },
        { "DKK", new ExchangeRate(new Currency("DKK", "Danske kroner"), 100.00m) }
    };

    // Exchange rates: amount of DKK required to purchase 100 in the mentioned currency

    public ExchangeRate? GetExchangeRate(string currencyIsoCode)
    {
        _rates.TryGetValue(currencyIsoCode, out var rate);
        return rate;
    }
}
