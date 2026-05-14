-- Drop existing database if it exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'ECommerceDb')
BEGIN
    ALTER DATABASE ECommerceDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDb;
END
GO

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
('LEGO CREATOR 3IN1 Flori in Stropitoare', 'SKU: LEGO31149 - Build 3 unique flower arrangements with this creative LEGO set.', 97.13, 'https://via.placeholder.com/300x300?text=LEGO31149', 3),
('LEGO FLOWERS Flori de Lotus', 'SKU: LEGO40647 - Beautiful lotus flowers for creative building and display.', 48.26, 'https://via.placeholder.com/300x300?text=LEGO40647', 4),
('LEGO CREATOR Masina de Scris cu Flori', 'SKU: LEGO31169 - Vintage typewriter with floral decorations - 3 in 1 design.', 97.07, 'https://via.placeholder.com/300x300?text=LEGO31169', 3),
('LEGO BOTANICALS Buchet de Lalele', 'SKU: LEGO11501 - Elegant tulip bouquet display set with botanical details.', 201.78, 'https://via.placeholder.com/300x300?text=LEGO11501', 1),
('LEGO CREATOR Colibri Colorat', 'SKU: LEGO31384 - Wild animal series featuring a vibrant hummingbird figure.', 96.92, 'https://via.placeholder.com/300x300?text=LEGO31384', 2),
('LEGO DUPLO Sortator de Forme Casa pentru Catei', 'SKU: LEGO10441 - Shape sorter playhouse perfect for toddlers and their furry friends.', 97.07, 'https://via.placeholder.com/300x300?text=LEGO10441', 2),
('LEGO DUPLO Animalute de Companie Creative', 'SKU: LEGO10477 - 3 in 1 creative pet building set for ages 2-5.', 30.28, 'https://via.placeholder.com/300x300?text=LEGO10477', 2),
('PUZZLE Trefl 500 Tea Time', 'Premium Plus 500 piece puzzle with beautiful Manhattan Bridge European championship design.', 21.63, 'https://via.placeholder.com/300x300?text=TREFL500', 2);
GO