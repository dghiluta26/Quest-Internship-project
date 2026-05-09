# E-Commerce Cart & Checkout API

This project is a simplified full-stack e-commerce vertical slice built with Angular, ASP.NET Core Web API, and SQL Server.

## Technologies Used

- ASP.NET Core Web API
- C#
- SQL Server
- Microsoft.Data.SqlClient
- Angular
- RxJS BehaviorSubject
- Bootstrap
- xUnit
- Moq

## Features

- User registration
- User login
- Product catalog
- Shopping cart
- Cart counter with instant updates
- Checkout form
- Backend-calculated order total
- SQL Server database
- Unit tests

## Important Business Rule

The checkout total is calculated by the backend using product prices from the SQL Server database.

The frontend sends only:

- product ID
- quantity
- shipping address
- user ID

The backend does not trust a total price sent by the frontend.

## Database Setup

The database is named:

```text
ECommerceDb