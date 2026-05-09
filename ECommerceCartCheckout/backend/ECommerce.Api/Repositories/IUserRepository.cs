using ECommerce.Api.Models;

namespace ECommerce.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<int> CreateAsync(User user);
}