using ECommerce.Api.DTOs;

namespace ECommerce.Api.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);
}