using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pricing.Api.Tests.TestDoubles;
using Pricing.Core;
using Pricing.Core.Tests.TestDoubles;

namespace Pricing.Api.Tests;

public class ApplyPricingEndpointSpecification
{
    private const string RequestUri = "PricingTable";
    
    [Fact]
    public async Task ShouldReturn500WhenCausesException()
    {
        using var client = CreateApiWithPricingManager<StubExceptionPricingManager>()
            .CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri, CreateRequest());
        
        response.Should().HaveStatusCode(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldReturn400WhenPricingManagerReturnFalse()
    {
        using var client = CreateApiWithPricingManager<StubExceptionPricingManager>()
            .CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri, CreateRequest());        

        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task ShouldReturn200WhenPricingManagerReturnFalse()
    {
        using var client = CreateApiWithPricingManager<StubExceptionPricingManager>()
            .CreateClient();

        var response = await client.PutAsJsonAsync(RequestUri, CreateRequest());
        
        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ShouldSendRequestToPricingManager()
    {
        var pricingStore = new InMemoryPricingStore();
        
        var api = new ApiFactory(services =>
        {
            services.TryAddScoped<IPricingStore>( _=> pricingStore);
        });
        var client = api.CreateClient();
        var applyPricingRequest = CreateRequest();

        var response = await client.PutAsJsonAsync(RequestUri, applyPricingRequest);

        pricingStore.GetPricingTable().Should().BeEquivalentTo(applyPricingRequest);
    }

    private static ApplyPricingRequest CreateRequest()
    {
        return new ApplyPricingRequest(
            new[] { new PriceTierRequest(24, 1) }
        );
    }

    private static ApiFactory CreateApiWithPricingManager<T>() 
        where T : class, IPricingManager
    {
        var api = new ApiFactory(services =>
        {
            services.RemoveAll(typeof(IPricingManager));
            
            services.TryAddScoped<IPricingManager, T>();
        });
        
        return api;
    }
}