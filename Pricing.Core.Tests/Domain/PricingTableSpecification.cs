using FluentAssertions;
using Pricing.Core.Domain;

namespace Pricing.Core.Tests.Domain;

public class PricingTableSpecification
{
    [Fact]
    public void ShouldThrowNullExceptionIfPriceTiersIsNull()
    {
        var create = () => new PricingTable(null!);

        create.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ShouldThrowAgrumentExceptionIfHasNotPriceTiers()
    {
        var create = () => new PricingTable(Array.Empty<PriceTier>());

        create.Should().ThrowExactly<ArgumentException>()
            .WithParameterName(nameof(PricingTable.Tiers))
            .WithMessage("Missing Pricing Tiers*");
        
    }

    [Fact]
    public void ShouldHaveOneTierWhenCreatedWithOne()
    {
        var pricingTier = new PricingTable(new[] { CreatePriceTier() });

        pricingTier.Tiers.Should().HaveCount(1);
    }

    [Fact]
    public void PriceTiersShouldBeOrderedByHourLimit()
    {
        var pricingTable = new PricingTable(new[]
        {
            CreatePriceTier(hourLimit: 24),
            CreatePriceTier(hourLimit: 4)
        });

        pricingTable.Tiers.Should().BeInAscendingOrder(tier => tier.HourLimit);
    }

    [Theory]
    [InlineData(2, 1, 25)]
    [InlineData(3, 2, 49)]
    public void MaximumDailyPriceShouldBeCalculatedUsingTiersIfNotDefined(decimal price1, decimal price2, 
        decimal maxPrice)
    {
        var pricingTable = new PricingTable(new[]
        {
            CreatePriceTier(hourLimit: 1, price: price1),
            CreatePriceTier(hourLimit: 24, price: price2)
        }, maxDaillyPrice: null);

        pricingTable.GetMaxDailyPrice().Should().Be(maxPrice);
    }

    [Fact]
    public void ShouldBeAbleToSetMaximumDailyPrice()
    {
        const int maxDailyPrice = 15;
        var pricingTable = new PricingTable(new[]
        {
            CreatePriceTier(hourLimit: 24)
        }, maxDaillyPrice: maxDailyPrice);

        pricingTable.GetMaxDailyPrice().Should().Be(maxDailyPrice);
    }

    [Fact]
    public void ShouldFailIfTiersDoNotCover24Hours()
    {
        var create = () => new PricingTable(new[]
        {
            CreatePriceTier(hourLimit: 20)
        });

        create.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ShouldFailIfMaxDailyPriceGtTiersPrice()
    {
        var create = () => new PricingTable(new[]
        {
            CreatePriceTier(hourLimit: 24, price: 1)
        }, maxDaillyPrice: 26);

        create.Should().Throw<ArgumentException>();
    }

    private static PriceTier CreatePriceTier(int hourLimit = 24, decimal price = 1) => new(hourLimit, price);
}