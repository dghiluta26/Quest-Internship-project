using ECommerce.Api.Models;

namespace ECommerce.Api.Repositories;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order, List<OrderItem> items);
}