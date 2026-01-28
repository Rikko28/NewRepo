using FxExchange.Core.Models;

namespace FxExchange.Tests.Models;

public class ExchangeRateTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesExchangeRate()
    {
        // Arrange
        var currency = new Currency("EUR", "Euro");

        // Act
        var rate = new ExchangeRate(currency, 743.94m);

        // Assert
        Assert.Equal(currency, rate.Currency);
        Assert.Equal(743.94m, rate.RateToDkk);
    }

    [Fact]
    public void Constructor_WithNullCurrency_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ExchangeRate(null!, 100m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.5)]
    public void Constructor_WithNonPositiveRate_ThrowsArgumentException(decimal rate)
    {
        // Arrange
        var currency = new Currency("EUR", "Euro");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ExchangeRate(currency, rate));
    }

    [Fact]
    public void Constructor_WithPositiveRate_CreatesExchangeRate()
    {
        // Arrange
        var currency = new Currency("EUR", "Euro");

        // Act
        var rate = new ExchangeRate(currency, 0.01m);

        // Assert
        Assert.Equal(0.01m, rate.RateToDkk);
    }
}
