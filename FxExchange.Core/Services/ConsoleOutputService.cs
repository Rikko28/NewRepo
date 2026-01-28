namespace FxExchange.Core.Services;

public class ConsoleOutputService(Action<string> writeLine) : IConsoleOutputService
{
    private readonly Action<string> _writeLine = writeLine ?? throw new ArgumentNullException(nameof(writeLine));

    public void ShowUsage()
    {
        _writeLine("Usage: Exchange <currency_pair> <amount>");
    }

    public void ShowResult(ExchangeCommandResult result)
    {
        if (result.IsSuccess)
        {
            _writeLine($"{result.ConvertedAmount:F4}");
            return;
        }

        if (result.IsCurrencyNotFound)
        {
            _writeLine($"Error: Currency '{result.NotFoundCurrencyCode}' is not supported.");
            _writeLine("");
            ShowSupportedCurrencies();
            return;
        }

        if (result.IsInvalidFormat)
        {
            _writeLine($"Error: {result.ErrorMessage}");
            return;
        }

        _writeLine($"Error: {result.ErrorMessage}");

        _writeLine("");
    }

    private void ShowSupportedCurrencies()
    {
        _writeLine("Supported currencies:");
        _writeLine("  EUR - Euro");
        _writeLine("  USD - Amerikanske dollar");
        _writeLine("  GBP - Britiske pund");
        _writeLine("  SEK - Svenske kroner");
        _writeLine("  NOK - Norske kroner");
        _writeLine("  CHF - Schweiziske franc");
        _writeLine("  JPY - Japanske yen");
        _writeLine("  DKK - Danske kroner");
    }
}