using FxExchange.Console;
using FxExchange.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace FxExchange.Tests.Console;

public class ExchangeApplicationTests
{
    private readonly Mock<IExchangeCommandProcessor> _commandProcessorMock = new();
    private readonly Mock<IConsoleOutputService> _outputServiceMock = new();

    [Fact]
    public void Constructor_WithNullProcessor_ThrowsArgumentNullException()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ExchangeApplication(null!, _outputServiceMock.Object, () => inputQueue.Dequeue()));
    }

    [Fact]
    public void Constructor_WithNullOutputService_ThrowsArgumentNullException()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ExchangeApplication(_commandProcessorMock.Object, null!, () => inputQueue.Dequeue()));
    }

    [Fact]
    public void Constructor_WithNullReadLine_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, null!));
    }

    [Fact]
    public void Run_ShowsUsageOnStart()
    {
        // Arrange
        var inputQueue = new Queue<string?>([null]);
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        _outputServiceMock.Verify(o => o.ShowUsage(), Times.Once);
    }

    [Fact]
    public void Run_ProcessesValidCommand()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);
        _commandProcessorMock.Setup(p => p.ProcessCommand("Exchange EUR/USD 100"))
            .Returns(ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        _commandProcessorMock.Verify(p => p.ProcessCommand("Exchange EUR/USD 100"), Times.Once);
        _outputServiceMock.Verify(o => o.ShowResult(It.Is<ExchangeCommandResult>(r =>
            r.IsSuccess && r.MainCurrencyCode == "EUR" && r.MoneyCurrencyCode == "USD")), Times.Once);
    }

    [Fact]
    public void Run_SkipsEmptyInput()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["", "   ", "Exchange EUR/USD 100", null]);
        _commandProcessorMock.Setup(p => p.ProcessCommand("Exchange EUR/USD 100"))
            .Returns(ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        _commandProcessorMock.Verify(p => p.ProcessCommand(It.IsAny<string>()), Times.Once);
        _commandProcessorMock.Verify(p => p.ProcessCommand("Exchange EUR/USD 100"), Times.Once);
    }

    [Fact]
    public void Run_ProcessesMultipleCommands()
    {
        // Arrange
        var inputQueue = new Queue<string?>([
            "Exchange EUR/USD 100",
            "Exchange GBP/DKK 50",
            "Exchange JPY/NOK 1000",
            null
        ]);
        _commandProcessorMock.Setup(p => p.ProcessCommand(It.IsAny<string>()))
            .Returns((string _) => ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        _commandProcessorMock.Verify(p => p.ProcessCommand("Exchange EUR/USD 100"), Times.Once);
        _commandProcessorMock.Verify(p => p.ProcessCommand("Exchange GBP/DKK 50"), Times.Once);
        _commandProcessorMock.Verify(p => p.ProcessCommand("Exchange JPY/NOK 1000"), Times.Once);
        _outputServiceMock.Verify(o => o.ShowResult(It.IsAny<ExchangeCommandResult>()), Times.Exactly(3));
    }

    [Fact]
    public void Run_HandlesInvalidCommand()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["Invalid Command", null]);
        _commandProcessorMock.Setup(p => p.ProcessCommand("Invalid Command"))
            .Returns(ExchangeCommandResult.InvalidFormat("Command must start with 'Exchange'"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        _commandProcessorMock.Verify(p => p.ProcessCommand("Invalid Command"), Times.Once);
        _outputServiceMock.Verify(o => o.ShowResult(It.Is<ExchangeCommandResult>(r =>
            !r.IsSuccess && r.IsInvalidFormat)), Times.Once);
    }

    [Fact]
    public void Run_ExitsOnNullInput()
    {
        // Arrange
        var callCount = 0;
        var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);
        _commandProcessorMock.Setup(p => p.ProcessCommand(It.IsAny<string>()))
            .Returns(ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () =>
        {
            callCount++;
            return inputQueue.Dequeue();
        });

        // Act
        app.Run();

        // Assert
        Assert.Equal(2, callCount);
        _commandProcessorMock.Verify(p => p.ProcessCommand(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void Run_CallsShowUsageBeforeProcessingCommands()
    {
        // Arrange
        var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);
        var callSequence = new List<string>();
        _outputServiceMock.Setup(o => o.ShowUsage()).Callback(() => callSequence.Add("ShowUsage"));
        _commandProcessorMock.Setup(p => p.ProcessCommand(It.IsAny<string>()))
            .Callback(() => callSequence.Add("ProcessCommand"))
            .Returns(ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD"));
        var app = new ExchangeApplication(_commandProcessorMock.Object, _outputServiceMock.Object, () => inputQueue.Dequeue());

        // Act
        app.Run();

        // Assert
        Assert.Equal(2, callSequence.Count);
        Assert.Equal("ShowUsage", callSequence[0]);
        Assert.Equal("ProcessCommand", callSequence[1]);
    }

    [Fact]
    public void Run_WithDependencyInjection_IntegrationTest()
    {
        // Arrange
        var output = new List<string>();
        var testServiceProvider = new ServiceCollection()
            .AddSingleton<IExchangeRateProvider, HardcodedExchangeRateProvider>()
            .AddSingleton<ICurrencyConverter, CurrencyConverter>()
            .AddSingleton<IExchangeCommandProcessor, ExchangeCommandProcessor>()
            .AddSingleton<IConsoleOutputService>(_ => new ConsoleOutputService(line => output.Add(line)))
            .AddSingleton<Func<string?>>(_ =>
            {
                var inputQueue = new Queue<string?>(["Exchange EUR/USD 100", null]);
                return () => inputQueue.Dequeue();
            })
            .AddSingleton<ExchangeApplication>()
            .BuildServiceProvider();

        // Act
        var app = testServiceProvider.GetRequiredService<ExchangeApplication>();
        app.Run();

        // Assert
        Assert.Contains("Usage: Exchange <currency_pair> <amount>", output);
        Assert.Contains("112.1895", output);
    }
}