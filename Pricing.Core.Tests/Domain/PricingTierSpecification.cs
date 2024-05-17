using FluentAssertions;
using Pricing.Core.Domain;
using Pricing.Core.Exceptions;

namespace Pricing.Core.Tests.Domain;

public class PricingTierSpecification
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(25, 1)]
    [InlineData(-1, 1)]
    public void ShouldThrowInvalidPricingTierWhenHourLimitIsInvalid(int hour, decimal price)
    {
        var create = () => new PriceTier(hour, price);

        create.Should().ThrowExactly<InvalidPricingTierException>();
    }

    [Fact]
    public void ShouldThrowInvalidPricingTierExceptionWhenPriceIsNegative()
    {
        var create = () => new PriceTier(1, -1);

        create.Should().Throw<InvalidPricingTierException>();
    }
}
