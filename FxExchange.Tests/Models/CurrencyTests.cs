using FxExchange.Core.Models;

namespace FxExchange.Tests.Models;

public class CurrencyTests
{
    [Fact]
    public void Constructor_WithValidParameters_CreatesCurrency()
    {
        // Act
        var currency = new Currency("USD", "US Dollar");

        // Assert
        Assert.Equal("USD", currency.IsoCode);
        Assert.Equal("US Dollar", currency.Name);
    }

    [Fact]
    public void Constructor_ConvertsIsoCodeToUpperCase()
    {
        // Act
        var currency = new Currency("usd", "US Dollar");

        // Assert
        Assert.Equal("USD", currency.IsoCode);
    }

    [Fact]
    public void Constructor_WithNullIsoCode_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency(null!, "US Dollar"));
    }

    [Fact]
    public void Constructor_WithEmptyIsoCode_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency("", "US Dollar"));
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Currency("USD", null!));
    }

    [Fact]
    public void Equals_WithSameIsoCode_ReturnsTrue()
    {
        // Arrange
        var currency1 = new Currency("USD", "US Dollar");
        var currency2 = new Currency("USD", "Different Name");

        // Act & Assert
        Assert.Equal(currency1, currency2);
    }

    [Fact]
    public void Equals_WithDifferentIsoCode_ReturnsFalse()
    {
        // Arrange
        var currency1 = new Currency("USD", "US Dollar");
        var currency2 = new Currency("EUR", "Euro");

        // Act & Assert
        Assert.NotEqual(currency1, currency2);
    }

    [Fact]
    public void ToString_ReturnsIsoCode()
    {
        // Arrange
        var currency = new Currency("USD", "US Dollar");

        // Act
        var result = currency.ToString();

        // Assert
        Assert.Equal("USD", result);
    }
}
