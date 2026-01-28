using FxExchange.Core.Exceptions;
using FxExchange.Core.Models;
using FxExchange.Core.Services;
using Moq;

namespace FxExchange.Tests.Services;

public class CurrencyConverterTests
{
    private readonly Mock<IExchangeRateProvider> _exchangeRateProviderMock;
    private readonly CurrencyConverter _converter;

    public CurrencyConverterTests()
    {
        _exchangeRateProviderMock = new Mock<IExchangeRateProvider>();
        _converter = new CurrencyConverter(_exchangeRateProviderMock.Object);
    }

    [Fact]
    public void Constructor_WithNullProvider_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CurrencyConverter(null!));
    }

    [Fact]
    public void Convert_WithNullCurrencyPair_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _converter.Convert(null!, 100m));
    }

    [Fact]
    public void Convert_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        var pair = CurrencyPair.Parse("EUR/USD");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _converter.Convert(pair, -100m));
    }

    [Theory]
    [InlineData("EUR/EUR", 100)]
    [InlineData("USD/USD", 50.5)]
    [InlineData("DKK/DKK", 1000)]
    public void Convert_WithSameCurrency_ReturnsOriginalAmount(string pairString, double amountDouble)
    {
        // Arrange
        var amount = (decimal)amountDouble;
        var pair = CurrencyPair.Parse(pairString);

        // Act
        var result = _converter.Convert(pair, amount);

        // Assert
        Assert.Equal(amount, result);
        _exchangeRateProviderMock.Verify(p => p.GetExchangeRate(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Convert_EurToUsd_ReturnsCorrectAmount()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("USD"))
            .Returns(new ExchangeRate(new Currency("USD", "US Dollar"), 663.11m));
        var pair = CurrencyPair.Parse("EUR/USD");

        // Act
        var result = _converter.Convert(pair, 100m);

        // Assert
        Assert.Equal(112.19m, result, 2);
        _exchangeRateProviderMock.Verify(p => p.GetExchangeRate("EUR"), Times.Once);
        _exchangeRateProviderMock.Verify(p => p.GetExchangeRate("USD"), Times.Once);
    }

    [Fact]
    public void Convert_UsdToEur_ReturnsCorrectAmount()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("USD"))
            .Returns(new ExchangeRate(new Currency("USD", "US Dollar"), 663.11m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        var pair = CurrencyPair.Parse("USD/EUR");

        // Act
        var result = _converter.Convert(pair, 100m);

        // Assert
        Assert.Equal(89.13m, result, 2);
    }

    [Fact]
    public void Convert_GbpToDkk_ReturnsCorrectAmount()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("GBP"))
            .Returns(new ExchangeRate(new Currency("GBP", "British Pound"), 852.85m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("DKK"))
            .Returns(new ExchangeRate(new Currency("DKK", "Danish Krone"), 100m));
        var pair = CurrencyPair.Parse("GBP/DKK");

        // Act
        var result = _converter.Convert(pair, 100m);

        // Assert
        Assert.Equal(852.85m, result, 2);
    }

    [Fact]
    public void Convert_DkkToEur_ReturnsCorrectAmount()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("DKK"))
            .Returns(new ExchangeRate(new Currency("DKK", "Danish Krone"), 100m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        var pair = CurrencyPair.Parse("DKK/EUR");

        // Act
        var result = _converter.Convert(pair, 743.94m);

        // Assert
        Assert.Equal(100m, result, 2);
    }

    [Fact]
    public void Convert_SekToNok_ReturnsCorrectAmount()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("SEK"))
            .Returns(new ExchangeRate(new Currency("SEK", "Swedish Krona"), 76.10m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("NOK"))
            .Returns(new ExchangeRate(new Currency("NOK", "Norwegian Krone"), 78.40m));
        var pair = CurrencyPair.Parse("SEK/NOK");

        // Act
        var result = _converter.Convert(pair, 100m);

        // Assert
        Assert.Equal(97.07m, result, 2);
    }

    [Fact]
    public void Convert_WithZeroAmount_ReturnsZero()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("USD"))
            .Returns(new ExchangeRate(new Currency("USD", "US Dollar"), 663.11m));
        var pair = CurrencyPair.Parse("EUR/USD");

        // Act
        var result = _converter.Convert(pair, 0m);

        // Assert
        Assert.Equal(0m, result);
        _exchangeRateProviderMock.Verify(p => p.GetExchangeRate("EUR"), Times.Once);
        _exchangeRateProviderMock.Verify(p => p.GetExchangeRate("USD"), Times.Once);
    }

    [Fact]
    public void Convert_WithDecimalAmount_HandlesCorrectly()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("USD"))
            .Returns(new ExchangeRate(new Currency("USD", "US Dollar"), 663.11m));
        var pair = CurrencyPair.Parse("EUR/USD");

        // Act
        var result = _converter.Convert(pair, 50.5m);

        // Assert
        Assert.True(result > 0);
        Assert.Equal(56.66m, result, 2);
    }

    [Fact]
    public void Convert_WithUnknownMainCurrency_ThrowsCurrencyNotFoundException()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("XYZ"))
            .Throws(new CurrencyNotFoundException("XYZ"));
        var mainCurrency = new Currency("XYZ", "Unknown");
        var moneyCurrency = new Currency("USD", "US Dollar");
        var pair = new CurrencyPair(mainCurrency, moneyCurrency);

        // Act & Assert
        var exception = Assert.Throws<CurrencyNotFoundException>(() => _converter.Convert(pair, 100m));
        Assert.Equal("XYZ", exception.CurrencyIsoCode);
    }

    [Fact]
    public void Convert_WithUnknownMoneyCurrency_ThrowsCurrencyNotFoundException()
    {
        // Arrange
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("ABC"))
            .Throws(new CurrencyNotFoundException("ABC"));
        var mainCurrency = new Currency("EUR", "Euro");
        var moneyCurrency = new Currency("ABC", "Unknown");
        var pair = new CurrencyPair(mainCurrency, moneyCurrency);

        // Act & Assert
        var exception = Assert.Throws<CurrencyNotFoundException>(() => _converter.Convert(pair, 100m));
        Assert.Equal("ABC", exception.CurrencyIsoCode);
    }

    [Theory]
    [InlineData("JPY/EUR", 10000, 80.25)]
    [InlineData("CHF/GBP", 100, 80.13)]
    public void Convert_VariousCurrencyPairs_ReturnsReasonableResults(string pairString, double amountDouble, double expectedApproxDouble)
    {
        // Arrange
        var amount = (decimal)amountDouble;
        var expectedApprox = (decimal)expectedApproxDouble;
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("JPY"))
            .Returns(new ExchangeRate(new Currency("JPY", "Japanese Yen"), 5.96m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("EUR"))
            .Returns(new ExchangeRate(new Currency("EUR", "Euro"), 743.94m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("CHF"))
            .Returns(new ExchangeRate(new Currency("CHF", "Swiss Franc"), 683.39m));
        _exchangeRateProviderMock.Setup(p => p.GetExchangeRate("GBP"))
            .Returns(new ExchangeRate(new Currency("GBP", "British Pound"), 852.85m));
        var pair = CurrencyPair.Parse(pairString);

        // Act
        var result = _converter.Convert(pair, amount);

        // Assert
        Assert.True(result > 0);
        Assert.InRange(result, expectedApprox * 0.99m, expectedApprox * 1.01m);
    }

    [Fact]
    public void Convert_WithRealProvider_IntegrationTest()
    {
        // Arrange
        var exchangeRateProvider = new HardcodedExchangeRateProvider();
        var converter = new CurrencyConverter(exchangeRateProvider);
        var pair = CurrencyPair.Parse("EUR/USD");

        // Act
        var result = converter.Convert(pair, 100m);

        // Assert
        Assert.Equal(112.19m, result, 2);
    }
}
