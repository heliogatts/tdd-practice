using Pricing.Core.Tests;

namespace Pricing.Core;

public class PricingManager
{
    private readonly IPricingStore _pricingStore;

    public PricingManager(IPricingStore pricingStore)
    {
        _pricingStore = pricingStore;
    }
    public async Task<bool> HandleAsync(ApplyPricingRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        return await _pricingStore.SaveAsync(null!, cancellationToken);
    }
}