namespace FxExchange.Core.Models;

public class Currency
{
    public string IsoCode { get; }
    public string Name { get; }

    public Currency(string isoCode, string name)
    {
        if (string.IsNullOrWhiteSpace(isoCode))
            throw new ArgumentException("ISO code cannot be null or empty", nameof(isoCode));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        IsoCode = isoCode.ToUpperInvariant();
        Name = name;
    }

    public override bool Equals(object? obj)
    {
        return obj is Currency currency && IsoCode == currency.IsoCode;
    }

    public override int GetHashCode()
    {
        return IsoCode.GetHashCode();
    }

    public override string ToString()
    {
        return IsoCode;
    }
}
