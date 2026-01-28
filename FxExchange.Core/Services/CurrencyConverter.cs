using FxExchange.Core.Exceptions;
using FxExchange.Core.Models;

namespace FxExchange.Core.Services;

public class CurrencyConverter(IExchangeRateProvider exchangeRateProvider) : ICurrencyConverter
{
    private readonly IExchangeRateProvider _exchangeRateProvider = exchangeRateProvider ?? throw new ArgumentNullException(nameof(exchangeRateProvider));

    public decimal Convert(CurrencyPair currencyPair, decimal amount)
    {
        ArgumentNullException.ThrowIfNull(currencyPair);

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        // If both currencies are the same, return the amount as is
        if (currencyPair.MainCurrency.IsoCode == currencyPair.MoneyCurrency.IsoCode)
            return amount;

        // Get exchange rates for both currencies
        var mainRate = _exchangeRateProvider.GetExchangeRate(currencyPair.MainCurrency.IsoCode);
        if (mainRate == null)
            throw new CurrencyNotFoundException(currencyPair.MainCurrency.IsoCode);

        var moneyRate = _exchangeRateProvider.GetExchangeRate(currencyPair.MoneyCurrency.IsoCode);
        if (moneyRate == null)
            throw new CurrencyNotFoundException(currencyPair.MoneyCurrency.IsoCode);
        
        var dkkAmount = (amount / 100m) * mainRate.RateToDkk;
        var result = dkkAmount / (moneyRate.RateToDkk / 100m);

        return result;
    }
}
