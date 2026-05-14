using ECommerce.Api.Models;
using Microsoft.Data.SqlClient;

namespace ECommerce.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = """
            SELECT Id, FullName, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Email = @Email
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Email", email);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = """
            SELECT Id, FullName, Email, PasswordHash, CreatedAt
            FROM Users
            WHERE Id = @Id
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Email = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        };
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        const string query = """
            INSERT INTO Users (FullName, Email, PasswordHash)
            OUTPUT INSERTED.Id
            VALUES (@FullName, @Email, @PasswordHash)
            """;

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }
}