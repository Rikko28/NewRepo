using FxExchange.Console;
using FxExchange.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FxExchange.Tests.Console;

public class ProgramTests
{
    [Fact]
    public void ConfigureServices_RegistersAllRequiredServices()
    {
        // Act
        var serviceProvider = Program.ConfigureServices();

        // Assert
        var exchangeRateProvider = serviceProvider.GetService(typeof(IExchangeRateProvider));
        var converter = serviceProvider.GetService(typeof(ICurrencyConverter));
        var processor = serviceProvider.GetService(typeof(IExchangeCommandProcessor));
        var outputService = serviceProvider.GetService(typeof(IConsoleOutputService));
        var readLine = serviceProvider.GetService(typeof(Func<string?>));
        var app = serviceProvider.GetService(typeof(ExchangeApplication));

        Assert.NotNull(exchangeRateProvider);
        Assert.NotNull(converter);
        Assert.NotNull(processor);
        Assert.NotNull(outputService);
        Assert.NotNull(readLine);
        Assert.NotNull(app);

        Assert.IsType<HardcodedExchangeRateProvider>(exchangeRateProvider);
        Assert.IsType<CurrencyConverter>(converter);
        Assert.IsType<ExchangeCommandProcessor>(processor);
        Assert.IsType<ConsoleOutputService>(outputService);
        Assert.IsType<ExchangeApplication>(app);
    }

    [Fact]
    public void ConfigureServices_ReturnsValidServiceProvider()
    {
        // Act
        var serviceProvider = Program.ConfigureServices();

        // Assert
        Assert.NotNull(serviceProvider);
    }

    [Fact]
    public void ConfigureServices_CanResolveExchangeApplication()
    {
        // Act
        var serviceProvider = Program.ConfigureServices();
        var app = serviceProvider.GetRequiredService<ExchangeApplication>();

        // Assert
        Assert.NotNull(app);
    }

    [Fact]
    public void ConfigureServices_AllDependenciesCanBeResolved()
    {
        // Act
        var serviceProvider = Program.ConfigureServices();
        var processor = serviceProvider.GetRequiredService<IExchangeCommandProcessor>();
        var outputService = serviceProvider.GetRequiredService<IConsoleOutputService>();
        var readLine = serviceProvider.GetRequiredService<Func<string?>>();
        var app = serviceProvider.GetRequiredService<ExchangeApplication>();

        // Assert
        Assert.NotNull(processor);
        Assert.NotNull(outputService);
        Assert.NotNull(readLine);
        Assert.NotNull(app);
    }
}
