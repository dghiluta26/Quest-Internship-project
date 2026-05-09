-- ECommerceDb Backup - Created 2026-05-09 14:17:52
-- This is a backup of the database schema and initial data

CREATE DATABASE ECommerceDb_Backup;
GO

USE ECommerceDb_Backup;
GO

-- Users table structure
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- Products table structure  
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    Stock INT NOT NULL
);

-- Orders table structure
CREATE TABLE Orders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ShippingAddress NVARCHAR(500) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId)
    REFERENCES Users(Id)
);

-- OrderItems table structure
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

USE master;
GO

DROP DATABASE ECommerceDb_Backup;
GO
