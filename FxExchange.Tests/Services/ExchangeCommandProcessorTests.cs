using FxExchange.Core.Exceptions;
using FxExchange.Core.Models;
using FxExchange.Core.Services;
using Moq;

namespace FxExchange.Tests.Services;

public class ExchangeCommandProcessorTests
{
    private readonly Mock<ICurrencyConverter> _currencyConverterMock;
    private readonly ExchangeCommandProcessor _processor;

    public ExchangeCommandProcessorTests()
    {
        _currencyConverterMock = new Mock<ICurrencyConverter>();
        _processor = new ExchangeCommandProcessor(_currencyConverterMock.Object);
    }

    [Fact]
    public void Constructor_WithNullConverter_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ExchangeCommandProcessor(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ProcessCommand_WithNullOrWhiteSpaceInput_ReturnsInvalidFormatResult(string input)
    {
        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsInvalidFormat);
        Assert.Equal("Input cannot be empty", result.ErrorMessage);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData("Exchange")]
    [InlineData("Exchange EUR/USD")]
    [InlineData("Exchange EUR/USD 100 extra")]
    public void ProcessCommand_WithIncorrectNumberOfParts_ReturnsInvalidFormatResult(string input)
    {
        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsInvalidFormat);
        Assert.Equal("Invalid command format. Use: Exchange <currency_pair> <amount>", result.ErrorMessage);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData("Convert EUR/USD 100")]
    [InlineData("Trade EUR/USD 100")]
    public void ProcessCommand_WithIncorrectCommandName_ReturnsInvalidFormatResult(string input)
    {
        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsInvalidFormat);
        Assert.Equal("Command must start with 'Exchange'", result.ErrorMessage);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public void ProcessCommand_WithLowercaseExchange_IsSuccessful()
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()))
            .Returns(112.19m);

        // Act
        var result = _processor.ProcessCommand("exchange EUR/USD 100");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100m, result.OriginalAmount);
        Assert.Equal("EUR", result.MainCurrencyCode);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), 100m), Times.Once);
    }

    [Theory]
    [InlineData("Exchange EUR/USD abc")]
    [InlineData("Exchange EUR/USD 12.34.56")]
    [InlineData("Exchange EUR/USD not-a-number")]
    public void ProcessCommand_WithInvalidAmount_ReturnsInvalidFormatResult(string input)
    {
        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsInvalidFormat);
        Assert.Contains("Invalid amount", result.ErrorMessage);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData("Exchange EURUSD 100")]
    [InlineData("Exchange EUR-USD 100")]
    [InlineData("Exchange EUR 100")]
    public void ProcessCommand_WithInvalidCurrencyPairFormat_ReturnsInvalidFormatResult(string input)
    {
        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsInvalidFormat);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Never);
    }

    [Theory]
    [InlineData("Exchange XYZ/USD 100", "XYZ")]
    [InlineData("Exchange EUR/ABC 100", "ABC")]
    public void ProcessCommand_WithUnknownCurrency_ReturnsCurrencyNotFoundResult(string input, string expectedCurrencyCode)
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()))
            .Throws(new CurrencyNotFoundException(expectedCurrencyCode));

        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsCurrencyNotFound);
        Assert.Equal(expectedCurrencyCode, result.NotFoundCurrencyCode);
    }

    [Theory]
    [InlineData("Exchange EUR/USD 100", 100, 112.19)]
    [InlineData("Exchange GBP/DKK 50.25", 50.25, 428.56)]
    [InlineData("Exchange JPY/NOK 1000", 1000, 58.61)]
    public void ProcessCommand_WithValidInput_ReturnsSuccessResult(string input, double amountDouble, double expectedResultDouble)
    {
        // Arrange
        var amount = (decimal)amountDouble;
        var expectedResult = (decimal)expectedResultDouble;
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), amount))
            .Returns(expectedResult);

        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(amount, result.OriginalAmount);
        Assert.NotNull(result.MainCurrencyCode);
        Assert.Equal(expectedResult, result.ConvertedAmount);
        Assert.NotNull(result.MoneyCurrencyCode);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), amount), Times.Once);
    }

    [Fact]
    public void ProcessCommand_EurToUsd100_CallsConverterWithCorrectParameters()
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()))
            .Returns(112.19m);

        // Act
        var result = _processor.ProcessCommand("Exchange EUR/USD 100");

        // Assert
        Assert.True(result.IsSuccess);
        _currencyConverterMock.Verify(c => c.Convert(
            It.Is<CurrencyPair>(cp => cp.MainCurrency.IsoCode == "EUR" && cp.MoneyCurrency.IsoCode == "USD"),
            100m), Times.Once);
    }

    [Theory]
    [InlineData("Exchange EUR/EUR 100", 100)]
    [InlineData("Exchange USD/USD 50.5", 50.5)]
    [InlineData("Exchange DKK/DKK 1000", 1000)]
    public void ProcessCommand_WithSameCurrency_ReturnsOriginalAmount(string input, decimal expectedAmount)
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), expectedAmount))
            .Returns(expectedAmount);

        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedAmount, result.OriginalAmount);
        Assert.Equal(expectedAmount, result.ConvertedAmount);
    }

    [Fact]
    public void ProcessCommand_WithZeroAmount_ReturnsZeroConversion()
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), 0m))
            .Returns(0m);

        // Act
        var result = _processor.ProcessCommand("Exchange EUR/USD 0");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.OriginalAmount);
        Assert.Equal(0m, result.ConvertedAmount);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), 0m), Times.Once);
    }

    [Theory]
    [InlineData("Exchange  EUR/USD  100")]
    [InlineData("Exchange EUR/USD 100  ")]
    [InlineData("  Exchange EUR/USD 100")]
    public void ProcessCommand_WithExtraWhitespace_HandlesCorrectly(string input)
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), 100m))
            .Returns(112.19m);

        // Act
        var result = _processor.ProcessCommand(input);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100m, result.OriginalAmount);
        Assert.Equal("EUR", result.MainCurrencyCode);
        Assert.Equal("USD", result.MoneyCurrencyCode);
        _currencyConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), 100m), Times.Once);
    }

    [Fact]
    public void ProcessCommand_WhenConverterThrowsException_ReturnsErrorResult()
    {
        // Arrange
        _currencyConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()))
            .Throws(new InvalidOperationException("Conversion error"));

        // Act
        var result = _processor.ProcessCommand("Exchange EUR/USD 100");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Conversion error", result.ErrorMessage);
    }

    [Fact]
    public void ProcessCommand_WithValidInput_CallsConverterExactlyOnce()
    {
        // Arrange
        var strictConverterMock = new Mock<ICurrencyConverter>(MockBehavior.Strict);
        strictConverterMock.Setup(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()))
            .Returns(112.19m);
        var processor = new ExchangeCommandProcessor(strictConverterMock.Object);

        // Act
        processor.ProcessCommand("Exchange EUR/USD 100");

        // Assert
        strictConverterMock.Verify(c => c.Convert(It.IsAny<CurrencyPair>(), It.IsAny<decimal>()), Times.Once);
    }

    [Fact]
    public void ProcessCommand_WithRealConverter_IntegrationTest()
    {
        // Arrange
        var exchangeRateProvider = new HardcodedExchangeRateProvider();
        var converter = new CurrencyConverter(exchangeRateProvider);
        var processor = new ExchangeCommandProcessor(converter);

        // Act
        var result = processor.ProcessCommand("Exchange EUR/USD 100");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(100m, result.OriginalAmount);
        Assert.Equal("EUR", result.MainCurrencyCode);
        Assert.Equal("USD", result.MoneyCurrencyCode);
        Assert.Equal(112.19m, Math.Round(result.ConvertedAmount!.Value, 2));
    }
}