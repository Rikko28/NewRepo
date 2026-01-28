using FxExchange.Core.Services;
using Moq;

namespace FxExchange.Tests.Services;

public class ConsoleOutputServiceTests
{
    private readonly List<string> _output;
    private readonly ConsoleOutputService _service;
    private readonly Mock<Action<string>> _writeLineMock;

    public ConsoleOutputServiceTests()
    {
        _writeLineMock = new Mock<Action<string>>();
        _output = [];
        _service = new ConsoleOutputService(line => _output.Add(line));
    }

    [Fact]
    public void Constructor_WithNullWriteLine_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ConsoleOutputService(null!));
    }

    [Fact]
    public void ShowUsage_WritesCorrectMessage()
    {
        // Arrange
        var serviceWithMock = new ConsoleOutputService(_writeLineMock.Object);

        // Act
        serviceWithMock.ShowUsage();

        // Assert
        _writeLineMock.Verify(w => w("Usage: Exchange <currency_pair> <amount>"), Times.Once);
    }

    [Fact]
    public void ShowResult_WithSuccessResult_WritesFormattedConversion()
    {
        // Arrange
        var result = ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD");

        // Act
        _service.ShowResult(result);

        // Assert
        Assert.Equal(2, _output.Count);
        Assert.Equal("100.00 EUR = 112.1900 USD", _output[0]);
        Assert.Equal("", _output[1]);
    }

    [Fact]
    public void ShowResult_WithSuccessResult_CallsWriteLineTwice()
    {
        // Arrange
        var serviceWithMock = new ConsoleOutputService(_writeLineMock.Object);
        var result = ExchangeCommandResult.Success(100m, "EUR", 112.19m, "USD");

        // Act
        serviceWithMock.ShowResult(result);

        // Assert
        _writeLineMock.Verify(w => w(It.IsAny<string>()), Times.Exactly(2));
        _writeLineMock.Verify(w => w("100.00 EUR = 112.1900 USD"), Times.Once);
        _writeLineMock.Verify(w => w(""), Times.Once);
    }

    [Fact]
    public void ShowResult_WithInvalidFormatResult_WritesErrorMessage()
    {
        // Arrange
        var result = ExchangeCommandResult.InvalidFormat("Invalid command format");

        // Act
        _service.ShowResult(result);

        // Assert
        Assert.Equal(2, _output.Count);
        Assert.Equal("Error: Invalid command format", _output[0]);
        Assert.Equal("", _output[1]);
    }

    [Fact]
    public void ShowResult_WithInvalidFormatResult_CallsWriteLineTwice()
    {
        // Arrange
        var serviceWithMock = new ConsoleOutputService(_writeLineMock.Object);
        var result = ExchangeCommandResult.InvalidFormat("Invalid command format");

        // Act
        serviceWithMock.ShowResult(result);

        // Assert
        _writeLineMock.Verify(w => w(It.IsAny<string>()), Times.Exactly(2));
        _writeLineMock.Verify(w => w("Error: Invalid command format"), Times.Once);
        _writeLineMock.Verify(w => w(""), Times.Once);
    }

    [Fact]
    public void ShowResult_WithCurrencyNotFoundResult_WritesErrorAndSupportedCurrencies()
    {
        // Arrange
        var result = ExchangeCommandResult.CurrencyNotFound("XYZ");

        // Act
        _service.ShowResult(result);

        // Assert
        Assert.True(_output.Count > 10);
        Assert.Equal("Error: Currency 'XYZ' is not supported.", _output[0]);
        Assert.Equal("", _output[1]);
        Assert.Equal("Supported currencies:", _output[2]);
        Assert.Contains("  EUR - Euro", _output);
    }

    [Fact]
    public void ShowResult_WithCurrencyNotFoundResult_CallsShowSupportedCurrencies()
    {
        // Arrange
        var serviceWithMock = new ConsoleOutputService(_writeLineMock.Object);
        var result = ExchangeCommandResult.CurrencyNotFound("XYZ");

        // Act
        serviceWithMock.ShowResult(result);

        // Assert
        _writeLineMock.Verify(w => w(It.IsAny<string>()), Times.Exactly(12));
        _writeLineMock.Verify(w => w("Error: Currency 'XYZ' is not supported."), Times.Once);
        _writeLineMock.Verify(w => w("Supported currencies:"), Times.Once);
    }

    [Fact]
    public void ShowResult_WithGeneralError_WritesErrorMessage()
    {
        // Arrange
        var result = ExchangeCommandResult.Error("Something went wrong");

        // Act
        _service.ShowResult(result);

        // Assert
        Assert.Equal(2, _output.Count);
        Assert.Equal("Error: Something went wrong", _output[0]);
        Assert.Equal("", _output[1]);
    }

    [Theory]
    [InlineData(50.25, "GBP", 428.56, "DKK", "50.25 GBP = 428.5600 DKK")]
    [InlineData(1000, "JPY", 59.38, "EUR", "1000.00 JPY = 59.3800 EUR")]
    [InlineData(0.5, "CHF", 0.43, "USD", "0.50 CHF = 0.4300 USD")]
    public void ShowResult_WithVariousSuccessResults_FormatsCorrectly(
        decimal originalAmount,
        string mainCurrency,
        decimal convertedAmount,
        string moneyCurrency,
        string expectedOutput)
    {
        // Arrange
        var result = ExchangeCommandResult.Success(originalAmount, mainCurrency, convertedAmount, moneyCurrency);

        // Act
        _service.ShowResult(result);

        // Assert
        Assert.Equal(2, _output.Count);
        Assert.Equal(expectedOutput, _output[0]);
    }
}