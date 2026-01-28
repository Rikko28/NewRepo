using FxExchange.Core.Services;

namespace FxExchange.Console;

public class ExchangeApplication(
    IExchangeCommandProcessor processor,
    IConsoleOutputService outputService,
    Func<string?> readLine)
{
    private readonly IExchangeCommandProcessor _processor = processor ?? throw new ArgumentNullException(nameof(processor));
    private readonly IConsoleOutputService _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService));
    private readonly Func<string?> _readLine = readLine ?? throw new ArgumentNullException(nameof(readLine));

    public void Run()
    {
        _outputService.ShowUsage();

        while (true)
        {
            System.Console.Write("> ");
            var input = _readLine();

            if (input == null)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var result = _processor.ProcessCommand(input);
            _outputService.ShowResult(result);
        }
    }
}
