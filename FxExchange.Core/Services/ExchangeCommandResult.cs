namespace FxExchange.Core.Services;

public class ExchangeCommandResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? OriginalAmount { get; set; }
    public string? MainCurrencyCode { get; set; }
    public decimal? ConvertedAmount { get; set; }
    public string? MoneyCurrencyCode { get; set; }
    public bool IsInvalidFormat { get; set; }
    public bool IsCurrencyNotFound { get; set; }
    public string? NotFoundCurrencyCode { get; set; }

    public static ExchangeCommandResult Success(decimal originalAmount, string mainCurrency, decimal convertedAmount, string moneyCurrency)
    {
        return new ExchangeCommandResult
        {
            IsSuccess = true,
            OriginalAmount = originalAmount,
            MainCurrencyCode = mainCurrency,
            ConvertedAmount = convertedAmount,
            MoneyCurrencyCode = moneyCurrency
        };
    }

    public static ExchangeCommandResult InvalidFormat(string errorMessage)
    {
        return new ExchangeCommandResult
        {
            IsSuccess = false,
            IsInvalidFormat = true,
            ErrorMessage = errorMessage
        };
    }

    public static ExchangeCommandResult CurrencyNotFound(string currencyCode)
    {
        return new ExchangeCommandResult
        {
            IsSuccess = false,
            IsCurrencyNotFound = true,
            NotFoundCurrencyCode = currencyCode
        };
    }

    public static ExchangeCommandResult Error(string errorMessage)
    {
        return new ExchangeCommandResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
