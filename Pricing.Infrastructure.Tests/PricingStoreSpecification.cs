using System.Text.Json;
using Dapper;
using FluentAssertions;
using Pricing.Core;
using Pricing.Core.Domain;

namespace Pricing.Infrastructure.Tests;

public class PricingStoreSpecification : IClassFixture<PostgreSqlFixture>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PricingStoreSpecification(PostgreSqlFixture fixture)
    {
        _dbConnectionFactory = fixture.ConnectionFactory;
    }
    
    [Fact]
    public void ShouldThrowArgumentNullExceptionIfMissingConnectionString()
    {
        var create = () => new PostgresPricingStore(null!);
        
        create.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task ShouldReturnTrueWhenSaveWithSuccess()
    {
        IPricingStore store = new PostgresPricingStore(_dbConnectionFactory);
        var pricingTable = CreatePricingTable();
        
        var result = await store.SaveAsync(pricingTable, default);
        
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ShouldReplaceIfAlreadyExists()
    {
        IPricingStore store = new PostgresPricingStore(_dbConnectionFactory);
        await store.SaveAsync(CreatePricingTable(), default);
        var newPricingTable = new PricingTable(new[]
        {
            new PriceTier(24, 4)
        });
        
        var result = await store.SaveAsync(newPricingTable, default);
        
        result.Should().BeTrue();
        var data = await GetPricingFromStore();

        data.Should().HaveCount(1)
            .And
            .Subject
            .First().document.Equals(JsonSerializer.Serialize(newPricingTable));
    }

    private async Task<IEnumerable<dynamic>> GetPricingFromStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var data = await connection.QueryAsync(
            @"SELECT * FROM pricing");
        return data;
    }

    [Fact]
    public async Task ShouldInsertIfDoesNotExist()
    {
        IPricingStore store = new PostgresPricingStore(_dbConnectionFactory);
        var pricingTable = CreatePricingTable();
        await CleanUpPricingStore();

        var result = await store.SaveAsync(pricingTable, default);

        result.Should().BeTrue();
    }

    private async Task CleanUpPricingStore()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        await connection.ExecuteAsync("truncate table pricing");
    }

    private static PricingTable CreatePricingTable()
    {
        return new PricingTable( new []
        {
            new PriceTier(24, 0.5m)
        });
    }
}