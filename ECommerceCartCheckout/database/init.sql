CREATE DATABASE ECommerceDb;
GO

USE ECommerceDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    Stock INT NOT NULL
);
GO

CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ShippingAddress NVARCHAR(500) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId)
    REFERENCES Users(Id)
);
GO

CREATE TABLE OrderItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    LineTotal DECIMAL(10,2) NOT NULL,

    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId)
    REFERENCES Orders(Id),

    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId)
    REFERENCES Products(Id)
);
GO

INSERT INTO Products (Name, Description, Price, ImageUrl, Stock)
VALUES
('Blue Top', 'Casual blue women top.', 49.99, 'https://via.placeholder.com/300x300?text=Blue+Top', 20),
('Men T-Shirt', 'Simple cotton t-shirt.', 39.99, 'https://via.placeholder.com/300x300?text=T-Shirt', 30),
('Summer Dress', 'Light summer dress.', 89.99, 'https://via.placeholder.com/300x300?text=Dress', 15),
('Black Jeans', 'Regular fit black jeans.', 119.99, 'https://via.placeholder.com/300x300?text=Jeans', 12),
('Sneakers', 'Comfortable white sneakers.', 149.99, 'https://via.placeholder.com/300x300?text=Sneakers', 10),
('Handbag', 'Elegant casual handbag.', 199.99, 'https://via.placeholder.com/300x300?text=Handbag', 8),
('Jacket', 'Stylish light jacket.', 179.99, 'https://via.placeholder.com/300x300?text=Jacket', 10),
('Sunglasses', 'Modern sunglasses.', 69.99, 'https://via.placeholder.com/300x300?text=Sunglasses', 25);
GO