using FxExchange.Core.Exceptions;
using FxExchange.Core.Models;

namespace FxExchange.Core.Services;

public class ExchangeCommandProcessor(ICurrencyConverter converter) : IExchangeCommandProcessor
{
    private readonly ICurrencyConverter _converter = converter ?? throw new ArgumentNullException(nameof(converter));

    public ExchangeCommandResult ProcessCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return ExchangeCommandResult.InvalidFormat("Input cannot be empty");
        }

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return ExchangeCommandResult.InvalidFormat("Invalid command format. Use: Exchange <currency_pair> <amount>");
        }

        if (!parts[0].Equals("Exchange", StringComparison.OrdinalIgnoreCase))
        {
            return ExchangeCommandResult.InvalidFormat("Command must start with 'Exchange'");
        }

        var currencyPairInput = parts[1];
        var amountInput = parts[2];

        try
        {
            var currencyPair = CurrencyPair.Parse(currencyPairInput);

            if (!decimal.TryParse(amountInput, out var amount))
            {
                return ExchangeCommandResult.InvalidFormat($"Invalid amount '{amountInput}'. Please provide a valid decimal number.");
            }

            var result = _converter.Convert(currencyPair, amount);

            return ExchangeCommandResult.Success(
                amount,
                currencyPair.MainCurrency.IsoCode,
                result,
                currencyPair.MoneyCurrency.IsoCode
            );
        }
        catch (FormatException ex)
        {
            return ExchangeCommandResult.InvalidFormat(ex.Message);
        }
        catch (CurrencyNotFoundException ex)
        {
            return ExchangeCommandResult.CurrencyNotFound(ex.CurrencyIsoCode);
        }
        catch (Exception ex)
        {
            return ExchangeCommandResult.Error(ex.Message);
        }
    }
}
