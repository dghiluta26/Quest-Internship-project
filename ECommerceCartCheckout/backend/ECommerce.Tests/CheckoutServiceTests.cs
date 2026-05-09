using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using Moq;

namespace ECommerce.Tests;

public class CheckoutServiceTests
{
    [Fact]
    public async Task PlaceOrderAsync_CalculatesTotalUsingDatabaseProductPrices()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();

        userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new User
            {
                Id = 1,
                FullName = "Test User",
                Email = "test@test.com",
                PasswordHash = "hash"
            });

        productRepositoryMock
            .Setup(repo => repo.GetProductsByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Product 1",
                    Price = 50,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Product 2",
                    Price = 20,
                    Stock = 10
                }
            });

        orderRepositoryMock
            .Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
            .ReturnsAsync(100);

        var service = new CheckoutService(
            productRepositoryMock.Object,
            userRepositoryMock.Object,
            orderRepositoryMock.Object
        );

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Test address",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                },
                new CheckoutItemRequest
                {
                    ProductId = 2,
                    Quantity = 1
                }
            }
        };

        var result = await service.PlaceOrderAsync(request);

        Assert.Equal(100, result.OrderId);
        Assert.Equal(120, result.TotalPrice);
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsException_WhenCartIsEmpty()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();

        var service = new CheckoutService(
            productRepositoryMock.Object,
            userRepositoryMock.Object,
            orderRepositoryMock.Object
        );

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Test address",
            Items = new List<CheckoutItemRequest>()
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsException_WhenProductDoesNotExist()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();

        userRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new User
            {
                Id = 1,
                FullName = "Test User",
                Email = "test@test.com",
                PasswordHash = "hash"
            });

        productRepositoryMock
            .Setup(repo => repo.GetProductsByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Product>());

        var service = new CheckoutService(
            productRepositoryMock.Object,
            userRepositoryMock.Object,
            orderRepositoryMock.Object
        );

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Test address",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 999,
                    Quantity = 1
                }
            }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );
    }
}