using FxExchange.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FxExchange.Console;

internal class Program
{
    public static void Main()
    {
        var serviceProvider = ConfigureServices();

        var app = serviceProvider.GetRequiredService<ExchangeApplication>();
        app.Run();
    }

    internal static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IExchangeRateProvider, HardcodedExchangeRateProvider>();
        services.AddSingleton<ICurrencyConverter, CurrencyConverter>();
        services.AddSingleton<IExchangeCommandProcessor, ExchangeCommandProcessor>();
        services.AddSingleton<IConsoleOutputService>(_ =>
            new ConsoleOutputService(System.Console.WriteLine));
        services.AddSingleton<Func<string?>>(_ => System.Console.ReadLine);
        services.AddSingleton<ExchangeApplication>();

        return services.BuildServiceProvider();
    }
}