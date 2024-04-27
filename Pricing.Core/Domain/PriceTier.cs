namespace Pricing.Core.Domain;

public class PriceTier
{
    public PriceTier(int hourLimit, decimal price)
    {
        HourLimit = hourLimit;
        Price = price;
    }

    public int HourLimit { get; }
    public decimal Price { get; }
}