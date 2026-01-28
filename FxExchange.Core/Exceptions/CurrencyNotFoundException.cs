namespace FxExchange.Core.Exceptions;

public class CurrencyNotFoundException(string currencyIsoCode) : Exception($"Currency not found: {currencyIsoCode}")
{
    public string CurrencyIsoCode { get; } = currencyIsoCode;
}
