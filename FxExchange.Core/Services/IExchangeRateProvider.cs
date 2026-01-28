using FxExchange.Core.Models;

namespace FxExchange.Core.Services;

public interface IExchangeRateProvider
{
    ExchangeRate? GetExchangeRate(string currencyIsoCode);
}
