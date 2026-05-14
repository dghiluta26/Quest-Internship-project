using ECommerce.Api.Models;
using Microsoft.Data.SqlClient;

namespace ECommerce.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not found.");
    }

    public async Task<int> CreateOrderAsync(Order order, List<OrderItem> items)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            const string orderQuery = """
                INSERT INTO Orders (UserId, ShippingAddress, TotalPrice)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @ShippingAddress, @TotalPrice)
                """;

            using var orderCommand = new SqlCommand(orderQuery, connection, transaction);
            orderCommand.Parameters.AddWithValue("@UserId", order.UserId);
            orderCommand.Parameters.AddWithValue("@ShippingAddress", order.ShippingAddress);
            orderCommand.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);

            var orderIdObject = await orderCommand.ExecuteScalarAsync();
            int orderId = Convert.ToInt32(orderIdObject);

            foreach (var item in items)
            {
                const string stockQuery = """
                    UPDATE Products
                    SET Stock = Stock - @Quantity
                    WHERE Id = @ProductId
                        AND Stock >= @Quantity
                    """;

                using var stockCommand = new SqlCommand(stockQuery, connection, transaction);
                stockCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                stockCommand.Parameters.AddWithValue("@Quantity", item.Quantity);

                var affectedRows = await stockCommand.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException("One or more products do not have enough stock.");
                }

                const string itemQuery = """
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, LineTotal)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @LineTotal)
                    """;

                using var itemCommand = new SqlCommand(itemQuery, connection, transaction);
                itemCommand.Parameters.AddWithValue("@OrderId", orderId);
                itemCommand.Parameters.AddWithValue("@ProductId", item.ProductId);
                itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                itemCommand.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                itemCommand.Parameters.AddWithValue("@LineTotal", item.LineTotal);

                await itemCommand.ExecuteNonQueryAsync();
            }

            transaction.Commit();

            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
