using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using ECommerce.Api.Repositories;

namespace ECommerce.Api.Services;

public class CheckoutService : ICheckoutService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;

    public CheckoutService(
        IProductRepository productRepository,
        IUserRepository userRepository,
        IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    public async Task<CheckoutResponse> PlaceOrderAsync(CheckoutRequest request)
    {
        if (request.UserId <= 0)
        {
            throw new InvalidOperationException("Invalid user.");
        }

        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required.");
        }

        if (request.Items == null || request.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new InvalidOperationException("User does not exist.");
        }

        foreach (var item in request.Items)
        {
            if (item.ProductId <= 0)
            {
                throw new InvalidOperationException("Invalid product.");
            }

            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }
        }

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _productRepository.GetProductsByIdsAsync(productIds);

        if (products.Count != productIds.Count)
        {
            throw new InvalidOperationException("One or more products do not exist.");
        }

        var orderItems = new List<OrderItem>();
        decimal totalPrice = 0;

        foreach (var requestItem in request.Items)
        {
            var product = products.First(p => p.Id == requestItem.ProductId);

            if (requestItem.Quantity > product.Stock)
            {
                throw new InvalidOperationException($"Not enough stock for product {product.Name}.");
            }

            decimal lineTotal = product.Price * requestItem.Quantity;

            totalPrice += lineTotal;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = requestItem.Quantity,
                UnitPrice = product.Price,
                LineTotal = lineTotal
            });
        }

        var order = new Order
        {
            UserId = request.UserId,
            ShippingAddress = request.ShippingAddress,
            TotalPrice = totalPrice
        };

        int orderId = await _orderRepository.CreateOrderAsync(order, orderItems);

        return new CheckoutResponse
        {
            OrderId = orderId,
            TotalPrice = totalPrice,
            Message = "Order placed successfully."
        };
    }
}