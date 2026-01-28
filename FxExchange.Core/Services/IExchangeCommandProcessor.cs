namespace FxExchange.Core.Services;

public interface IExchangeCommandProcessor
{
    ExchangeCommandResult ProcessCommand(string input);
}
