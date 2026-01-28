namespace FxExchange.Core.Services;

public interface IConsoleOutputService
{
    void ShowUsage();
    void ShowResult(ExchangeCommandResult result);
}
