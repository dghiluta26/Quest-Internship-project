using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using Moq;

namespace ECommerce.Tests;

public class CheckoutServiceTests
{
    private static CheckoutService CreateService(
        Mock<IProductRepository> productRepositoryMock,
        Mock<IUserRepository> userRepositoryMock,
        Mock<IOrderRepository> orderRepositoryMock)
    {
        return new CheckoutService(
            productRepositoryMock.Object,
            userRepositoryMock.Object,
            orderRepositoryMock.Object
        );
    }

    [Fact]
    public async Task PlaceOrderAsync_CalculatesTotalUsingProductRepositoryPrices()
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
                    Name = "Blue Top",
                    Price = 50,
                    Stock = 10
                },
                new Product
                {
                    Id = 2,
                    Name = "Sneakers",
                    Price = 120,
                    Stock = 5
                }
            });

        orderRepositoryMock
            .Setup(repo => repo.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
            .ReturnsAsync(25);

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Craiova, Romania",
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

        Assert.Equal(25, result.OrderId);
        Assert.Equal(220, result.TotalPrice);
        Assert.Equal("Order placed successfully.", result.Message);

        orderRepositoryMock.Verify(repo =>
            repo.CreateOrderAsync(
                It.Is<Order>(order => order.TotalPrice == 220),
                It.Is<List<OrderItem>>(items =>
                    items.Count == 2 &&
                    items[0].LineTotal == 100 &&
                    items[1].LineTotal == 120
                )
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsException_WhenCartIsEmpty()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Craiova, Romania",
            Items = new List<CheckoutItemRequest>()
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );

        Assert.Equal("Cart is empty.", exception.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsException_WhenShippingAddressIsEmpty()
    {
        var productRepositoryMock = new Mock<IProductRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var orderRepositoryMock = new Mock<IOrderRepository>();

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 1
                }
            }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );

        Assert.Equal("Shipping address is required.", exception.Message);
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

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Craiova, Romania",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 999,
                    Quantity = 1
                }
            }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );

        Assert.Equal("One or more products do not exist.", exception.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsException_WhenQuantityIsGreaterThanStock()
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
                    Name = "Blue Top",
                    Price = 50,
                    Stock = 2
                }
            });

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Craiova, Romania",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 5
                }
            }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );

        Assert.Equal("Not enough stock for product Blue Top.", exception.Message);
    }

    [Fact]
    public async Task PlaceOrderAsync_CombinesDuplicateProductQuantitiesBeforeCheckingStock()
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
                    Name = "Blue Top",
                    Price = 50,
                    Stock = 3
                }
            });

        var service = CreateService(productRepositoryMock, userRepositoryMock, orderRepositoryMock);

        var request = new CheckoutRequest
        {
            UserId = 1,
            ShippingAddress = "Craiova, Romania",
            Items = new List<CheckoutItemRequest>
            {
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                },
                new CheckoutItemRequest
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.PlaceOrderAsync(request)
        );

        Assert.Equal("Not enough stock for product Blue Top.", exception.Message);
        orderRepositoryMock.Verify(
            repo => repo.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()),
            Times.Never
        );
    }
}
