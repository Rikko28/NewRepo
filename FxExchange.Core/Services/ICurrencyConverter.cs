using FxExchange.Core.Models;

namespace FxExchange.Core.Services;

public interface ICurrencyConverter
{
    decimal Convert(CurrencyPair currencyPair, decimal amount);
}
