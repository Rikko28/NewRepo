using FxExchange.Core.Services;

namespace FxExchange.Tests.Services;

public class HardcodedExchangeRateProviderTests
{
    private readonly HardcodedExchangeRateProvider _provider = new();

    [Theory]
    [InlineData("EUR", 743.94)]
    [InlineData("USD", 663.11)]
    [InlineData("GBP", 852.85)]
    [InlineData("SEK", 76.10)]
    [InlineData("NOK", 78.40)]
    [InlineData("CHF", 683.58)]
    [InlineData("JPY", 5.9740)]
    [InlineData("DKK", 100.00)]
    public void GetExchangeRate_WithValidCurrency_ReturnsCorrectRate(string isoCode, decimal expectedRate)
    {
        // Act
        var rate = _provider.GetExchangeRate(isoCode);

        // Assert
        Assert.NotNull(rate);
        Assert.Equal(isoCode, rate.Currency.IsoCode);
        Assert.Equal(expectedRate, rate.RateToDkk);
    }

    [Theory]
    [InlineData("eur")]
    [InlineData("EUR")]
    [InlineData("Eur")]
    public void GetExchangeRate_IsCaseInsensitive(string isoCode)
    {
        // Act
        var rate = _provider.GetExchangeRate(isoCode);

        // Assert
        Assert.NotNull(rate);
        Assert.Equal("EUR", rate.Currency.IsoCode);
    }

    [Theory]
    [InlineData("XYZ")]
    [InlineData("ABC")]
    [InlineData("")]
    public void GetExchangeRate_WithUnknownCurrency_ReturnsNull(string isoCode)
    {
        // Act
        var rate = _provider.GetExchangeRate(isoCode);

        // Assert
        Assert.Null(rate);
    }
}
