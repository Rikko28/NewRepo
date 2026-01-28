namespace FxExchange.Core.Models;

public class ExchangeRate
{
    public Currency Currency { get; }
    public decimal RateToDkk { get; }

    public ExchangeRate(Currency currency, decimal rateToDkk)
    {
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        
        if (rateToDkk <= 0)
            throw new ArgumentException("Exchange rate must be positive", nameof(rateToDkk));

        RateToDkk = rateToDkk;
    }
}
