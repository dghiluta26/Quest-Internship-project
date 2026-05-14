using ECommerce.Api.Models;

namespace ECommerce.Api.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<List<Product>> GetProductsByIdsAsync(List<int> ids);
}