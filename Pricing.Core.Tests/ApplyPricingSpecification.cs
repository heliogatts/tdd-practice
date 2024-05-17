using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Core.Tests;

public class ApplyPricingSpecification
{
    private readonly PricingManager _pricingManager;

    [Fact]
    public async Task ShouldThrowArgumentNullExceptionIfRequestIsNull()
    {
        var handleRequest = () => _pricingManager.HandleAsync(null!, default);
        await handleRequest.Should().ThrowAsync<NullReferenceException>();
    }

    [Fact]
    public async Task ShouldReturnTrueIfSucceeded()
    {
        var pricingManager = new PricingManager(new StubSuccessPricingStore());
        
        var result = await pricingManager.HandleAsync(new ApplyPricingRequest(), default);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseIfFailed()
    {
        var pricingManager = new PricingManager(new StubFailPricingStore());
        
        var result = await pricingManager.HandleAsync(new ApplyPricingRequest(), default);
        
        result.Should().BeFalse();
    }
}

public class DummyPricingStore : IPricingStore
{
    public Task<bool> SaveAsync(PricingTable pricingTable, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}