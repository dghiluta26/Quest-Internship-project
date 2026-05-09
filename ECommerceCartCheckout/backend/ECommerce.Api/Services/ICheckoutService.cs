using ECommerce.Api.DTOs;

namespace ECommerce.Api.Services;

public interface ICheckoutService
{
    Task<CheckoutResponse> PlaceOrderAsync(CheckoutRequest request);
}