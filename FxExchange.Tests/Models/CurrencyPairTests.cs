using FxExchange.Core.Models;

namespace FxExchange.Tests.Models;

public class CurrencyPairTests
{
    [Fact]
    public void Constructor_WithValidCurrencies_CreatesCurrencyPair()
    {
        // Arrange
        var mainCurrency = new Currency("EUR", "Euro");
        var moneyCurrency = new Currency("USD", "US Dollar");

        // Act
        var pair = new CurrencyPair(mainCurrency, moneyCurrency);

        // Assert
        Assert.Equal(mainCurrency, pair.MainCurrency);
        Assert.Equal(moneyCurrency, pair.MoneyCurrency);
    }

    [Fact]
    public void Constructor_WithNullMainCurrency_ThrowsArgumentNullException()
    {
        // Arrange
        var moneyCurrency = new Currency("USD", "US Dollar");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CurrencyPair(null!, moneyCurrency));
    }

    [Fact]
    public void Constructor_WithNullMoneyCurrency_ThrowsArgumentNullException()
    {
        // Arrange
        var mainCurrency = new Currency("EUR", "Euro");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CurrencyPair(mainCurrency, null!));
    }

    [Theory]
    [InlineData("EUR/USD")]
    [InlineData("eur/usd")]
    [InlineData("EUR / USD")]
    [InlineData(" EUR / USD ")]
    public void Parse_WithValidFormat_ParsesCurrencyPair(string input)
    {
        // Act
        var pair = CurrencyPair.Parse(input);

        // Assert
        Assert.Equal("EUR", pair.MainCurrency.IsoCode);
        Assert.Equal("USD", pair.MoneyCurrency.IsoCode);
    }

    [Fact]
    public void Parse_WithNull_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CurrencyPair.Parse(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_WithEmpty_ThrowsArgumentException(string input)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CurrencyPair.Parse(input));
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("EUR/")]
    [InlineData("/USD")]
    [InlineData("EUR/USD/GBP")]
    public void Parse_WithInvalidFormat_ThrowsFormatException(string input)
    {
        // Act & Assert
        Assert.Throws<FormatException>(() => CurrencyPair.Parse(input));
    }

    [Fact]
    public void ToString_ReturnsFormattedPair()
    {
        // Arrange
        var mainCurrency = new Currency("EUR", "Euro");
        var moneyCurrency = new Currency("USD", "US Dollar");
        var pair = new CurrencyPair(mainCurrency, moneyCurrency);

        // Act
        var result = pair.ToString();

        // Assert
        Assert.Equal("EUR/USD", result);
    }
}
