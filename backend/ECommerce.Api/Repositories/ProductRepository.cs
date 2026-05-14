using ECommerce.Api.Models;
using Microsoft.Data.SqlClient;

namespace ECommerce.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = """
            SELECT Id, Name, Description, Price, ImageUrl, Stock
            FROM Products
            ORDER BY Id
            """;

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                Stock = reader.GetInt32(5)
            });
        }

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = """
            SELECT Id, Name, Description, Price, ImageUrl, Stock
            FROM Products
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new Product
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            Price = reader.GetDecimal(3),
            ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
            Stock = reader.GetInt32(5)
        };
    }

    public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
    {
        var products = new List<Product>();

        if (ids.Count == 0)
        {
            return products;
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var parameterNames = ids.Select((id, index) => $"@Id{index}").ToList();

        string query = $"""
            SELECT Id, Name, Description, Price, ImageUrl, Stock
            FROM Products
            WHERE Id IN ({string.Join(",", parameterNames)})
            """;

        using var command = new SqlCommand(query, connection);

        for (int i = 0; i < ids.Count; i++)
        {
            command.Parameters.AddWithValue(parameterNames[i], ids[i]);
        }

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                Price = reader.GetDecimal(3),
                ImageUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                Stock = reader.GetInt32(5)
            });
        }

        return products;
    }
}