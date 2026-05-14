using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using ECommerce.Api.Repositories;
using ECommerce.Api.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ECommerce.Tests;

public class AuthServiceTests
{
    private static IConfiguration CreateConfiguration()
    {
        var values = new Dictionary<string, string?>
        {
            { "Jwt:Key", "THIS_IS_A_TEST_SECRET_KEY_123456789_ABCDEFG" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }

    private static AuthService CreateService(Mock<IUserRepository> userRepositoryMock)
    {
        return new AuthService(userRepositoryMock.Object, CreateConfiguration());
    }

    [Fact]
    public async Task RegisterAsync_ThrowsException_WhenEmailAlreadyExists()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                FullName = "Existing User",
                Email = "test@test.com",
                PasswordHash = "hash"
            });

        var service = CreateService(userRepositoryMock);

        var request = new RegisterRequest
        {
            FullName = "New User",
            Email = "test@test.com",
            Password = "Password123!"
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(request)
        );

        Assert.Equal("Email is already used.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserWithHashedPassword()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("new@test.com"))
            .ReturnsAsync((User?)null);

        userRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(1);

        var service = CreateService(userRepositoryMock);

        var request = new RegisterRequest
        {
            FullName = "New User",
            Email = "new@test.com",
            Password = "Password123!"
        };

        await service.RegisterAsync(request);

        userRepositoryMock.Verify(repo =>
            repo.CreateAsync(It.Is<User>(user =>
                user.FullName == "New User" &&
                user.Email == "new@test.com" &&
                user.PasswordHash != "Password123!" &&
                BCrypt.Net.BCrypt.Verify("Password123!", user.PasswordHash)
            )),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenUserDoesNotExist()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("missing@test.com"))
            .ReturnsAsync((User?)null);

        var service = CreateService(userRepositoryMock);

        var request = new LoginRequest
        {
            Email = "missing@test.com",
            Password = "Password123!"
        };

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(request)
        );

        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_ThrowsUnauthorized_WhenPasswordIsWrong()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123!");

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                FullName = "Test User",
                Email = "test@test.com",
                PasswordHash = hashedPassword
            });

        var service = CreateService(userRepositoryMock);

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "WrongPassword123!"
        };

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(request)
        );

        Assert.Equal("Invalid email or password.", exception.Message);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsException_WhenFullNameIsEmpty()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var service = CreateService(userRepositoryMock);

        var request = new RegisterRequest
        {
            FullName = "",
            Email = "test@test.com",
            Password = "Password123!"
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(request)
        );

        Assert.Equal("Full name is required.", exception.Message);
        userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsException_WhenPasswordIsEmpty()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var service = CreateService(userRepositoryMock);

        var request = new RegisterRequest
        {
            FullName = "Test User",
            Email = "test@test.com",
            Password = ""
        };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(request)
        );

        Assert.Equal("Password is required.", exception.Message);
        userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokenAndUserData_WhenCredentialsAreValid()
    {
        var userRepositoryMock = new Mock<IUserRepository>();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123!");

        userRepositoryMock
            .Setup(repo => repo.GetByEmailAsync("test@test.com"))
            .ReturnsAsync(new User
            {
                Id = 1,
                FullName = "Test User",
                Email = "test@test.com",
                PasswordHash = hashedPassword
            });

        var service = CreateService(userRepositoryMock);

        var request = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        var result = await service.LoginAsync(request);

        Assert.Equal(1, result.UserId);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("test@test.com", result.Email);
        Assert.False(string.IsNullOrWhiteSpace(result.Token));
    }
}