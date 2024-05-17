using FluentAssertions;
using NSubstitute;
using Pricing.Core.Domain;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Core.Tests;

public class ApplyPricingSpecification
{
    private readonly PricingManager _pricingManager = null!;
    private static int _hourLimit = 24;
    private static int _expectedPrice = 1;

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
        
        var result = await pricingManager.HandleAsync(CreateRequest(), default);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReturnFalseIfFailed()
    {
        var pricingManager = new PricingManager(new StubFailPricingStore());
        
        var result = await pricingManager.HandleAsync(CreateRequest(), default);
        
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldSaveOnlyOnce()
    {
        var spyPricingService = new SpyPricingService();
        var pricingManager = new PricingManager(spyPricingService);
        
        _ = await pricingManager.HandleAsync(CreateRequest(), default);
        
        spyPricingService.NumberOfSaves.Should().Be(_expectedPrice);
    }

    [Fact]
    public async Task ShouldSaveExpectedData()
    {
        var expectedPricingTable = new PricingTable(new [] { new PriceTier(_hourLimit, _expectedPrice) });
        var mockPricingStore = new MockPricingStore();
        mockPricingStore.ExpectedToSave(expectedPricingTable);
        var pricingManager = new PricingManager(mockPricingStore);
        
        _= await pricingManager.HandleAsync(CreateRequest(), default);
        
        mockPricingStore.Verify();
    }
    
    [Fact]
    public async Task ShouldSaveExpectedDataNSubstitute()
    {
        var expectedPricingTable = new PricingTable(new [] { new PriceTier(_hourLimit, _expectedPrice) });
        var mockPricingStore = Substitute.For<IPricingStore>();
            
        var pricingManager = new PricingManager(mockPricingStore);
        
        _= await pricingManager.HandleAsync(CreateRequest(), default);
        
        await mockPricingStore.Received().SaveAsync(
            Arg.Is<PricingTable>(
                table => table.Tiers.Count == expectedPricingTable.Tiers.Count), 
            default);
    }

    [Fact]
    public async Task ShouldSaveEquivalentDataToStorage()
    {
        var pricingStore = new InMemoryPricingStore();
        var pricingManager = new PricingManager(pricingStore);
        var applyPricingRequest = CreateRequest();

        _ = await pricingManager.HandleAsync(applyPricingRequest, default);

        pricingStore.GetPricingTable().Should().BeEquivalentTo(applyPricingRequest);
    }
    
    private static ApplyPricingRequest CreateRequest()
    {
        return new ApplyPricingRequest(new[] { new PriceTierRequest(_hourLimit, _expectedPrice) });
    }
}