using System.Text.Json;
using Dapper;
using Pricing.Core;
using Pricing.Core.Domain;

namespace Pricing.Infrastructure;

public class PostgresPricingStore : IPricingStore
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public PostgresPricingStore(IDbConnectionFactory dbConnectionFactory)
    {
        ArgumentNullException.ThrowIfNull(dbConnectionFactory);
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> SaveAsync(PricingTable pricingTable, CancellationToken cancellationToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        var data = new DbRecord(JsonSerializer.Serialize(pricingTable));

        var result = await connection.ExecuteAsync(
            @"INSERT INTO Pricing (key, document) VALUES (@key, @document) 
                    ON CONFLICT (key) DO UPDATE SET document = excluded.document;",
            data);

        return result > 0;
    }

    private record DbRecord(string Document, string Key = "ACTIVE");
}