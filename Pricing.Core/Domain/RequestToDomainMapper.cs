namespace Pricing.Core.Domain;

internal static class RequestToDomainMapper
{
    public static PricingTable ToPricingTable(this ApplyPricingRequest request)
    {
        return new PricingTable(
            request.Tiers.Select(x => new PriceTier(x.HourLimit, x.Price)));
    }
}