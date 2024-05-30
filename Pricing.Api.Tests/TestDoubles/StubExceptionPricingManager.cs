using Pricing.Core;
using Pricing.Core.Exceptions;

namespace Pricing.Api.Tests.TestDoubles;

public class StubExceptionPricingManager : IPricingManager
{
    public Task<bool> HandleAsync(ApplyPricingRequest request, CancellationToken cancellationToken)
    {
        throw new InvalidPricingTierException();
    }
}