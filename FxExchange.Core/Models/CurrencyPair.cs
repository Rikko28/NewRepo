namespace FxExchange.Core.Models;

public class CurrencyPair(Currency mainCurrency, Currency moneyCurrency)
{
    public Currency MainCurrency { get; } = mainCurrency ?? throw new ArgumentNullException(nameof(mainCurrency));
    public Currency MoneyCurrency { get; } = moneyCurrency ?? throw new ArgumentNullException(nameof(moneyCurrency));

    public static CurrencyPair Parse(string pair)
    {
        if (string.IsNullOrWhiteSpace(pair))
            throw new ArgumentException("Currency pair cannot be null or empty", nameof(pair));

        var parts = pair.Split('/');
        if (parts.Length != 2)
            throw new FormatException($"Invalid currency pair format: {pair}. Expected format: XXX/YYY");

        var mainIso = parts[0].Trim().ToUpperInvariant();
        var moneyIso = parts[1].Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(mainIso) || string.IsNullOrWhiteSpace(moneyIso))
            throw new FormatException($"Invalid currency pair format: {pair}");

        return new CurrencyPair(
            new Currency(mainIso, mainIso),
            new Currency(moneyIso, moneyIso)
        );
    }

    public override string ToString()
    {
        return $"{MainCurrency.IsoCode}/{MoneyCurrency.IsoCode}";
    }
}
