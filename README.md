# Shop4You — E-Commerce Cart & Checkout

A full-stack e-commerce application built with **Angular**, **ASP.NET Core Web API (.NET 10)**, and **SQL Server LocalDB**. Users can browse products, manage a persistent shopping cart with real-time stock enforcement, and place orders through a checkout flow backed by server-side price calculation.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 17+, Bootstrap, RxJS |
| Backend | ASP.NET Core Web API, .NET 10, C# |
| Database | SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) |
| Auth | JWT Bearer tokens, BCrypt password hashing |
| Testing | xUnit, Moq (backend) · Jasmine, Karma (frontend) |

---

## Features

- User registration and login with JWT authentication
- Product catalog with images served from local assets
- Stock enforcement on the product list, product detail, and cart pages
- Shopping cart persisted to `localStorage` — survives page refresh
- Cart badge with live item count in the navbar
- Checkout with server-side total calculation — the backend never trusts a price sent by the frontend
- Swagger UI available for API exploration
- Unit tests for backend services and repositories

---

## Prerequisites

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- [Node.js 18+ and npm](https://nodejs.org)
- **SQL Server LocalDB** — ships with Visual Studio; install via the [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/) (workload: *Data storage and processing*) or as a standalone [SQL Server Express LocalDB](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- `sqlcmd` — included with SQL Server / LocalDB installation
- **VS Code** with the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension (required for the one-click launch below)

---

## Quick Start — VS Code Launch

The repository includes pre-configured VS Code launch and task files. Open the repo root in VS Code, go to **Run and Debug** (`Ctrl+Shift+D`), and pick one of:

| Configuration | What it does |
|---|---|
| **Start Backend** | Builds and launches the API with the `coreclr` debugger, then opens Swagger at `https://localhost:7039/swagger/` |
| **Start Frontend** | Runs `npm install` then `ng serve`, then opens Chrome/Edge at `http://localhost:4200` |
| **Start Backend & Frontend** | Runs both above simultaneously |

> The database must be initialised before the first run — see **Database Setup** below.

---

## Manual Setup

### 1. Clone the Repository

```bash
git clone <your-repo-url>
cd Quest-Internship-project
```

### 2. Database Setup

Run the initialisation script against LocalDB. This creates the `Shop4YouDB` database, all tables, and seeds 8 products:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "database\init.sql"
```

Expected output:
```
Changed database context to 'Shop4YouDB'.
(8 rows affected)
```

Tables created: `Users`, `Products`, `Orders`, `OrderItems`

> If you need to reset the database at any point, re-running the same command will drop and recreate it cleanly.

### 3. Backend Setup

The connection string is pre-configured for LocalDB in `backend/ECommerce.Api/appsettings.json` — no changes needed for local development.

```bash
cd backend/ECommerce.Api
dotnet restore
dotnet run
```

The API starts at:
- **HTTPS:** `https://localhost:7039`
- **HTTP:** `http://localhost:5223`
- **Swagger UI:** `https://localhost:7039/swagger/`

### 4. Frontend Setup

```bash
cd frontend/ecommerce-client
npm install
npx ng serve
```

The app starts at `http://localhost:4200`.

---

## API Endpoints

### Authentication
| Method | Endpoint | Auth required |
|---|---|---|
| `POST` | `/api/auth/register` | No |
| `POST` | `/api/auth/login` | No |

### Products
| Method | Endpoint | Auth required |
|---|---|---|
| `GET` | `/api/products` | No |
| `GET` | `/api/products/{id}` | No |

### Checkout
| Method | Endpoint | Auth required |
|---|---|---|
| `POST` | `/api/checkout` | Yes (JWT) |

The checkout endpoint accepts only `productId`, `quantity`, and `shippingAddress`. The total price is always computed by the backend from the database — the frontend never sends a price.

---

## Running Tests

### Backend (xUnit + Moq)

```bash
cd backend/ECommerce.Tests
dotnet test
```

Covers `AuthService` and `CheckoutService` via mocked repositories:
- Registration validation (duplicate email, missing name, missing password)
- Login validation (user not found, wrong password, valid credentials → JWT issued)
- Checkout validation (invalid user ID, user not found, empty cart, missing shipping address, zero quantity, insufficient stock, duplicate product lines)
- Server-side total price calculation

### Frontend (Jasmine + Karma)

```bash
cd frontend/ecommerce-client
npx ng test
```

Covers `CartService`:
- Add to cart, quantity increment on duplicate, removal, clear
- Stock limit enforcement on `addToCart` and `increaseQuantity`
- `getCartQuantity` helper (in-cart and not-in-cart cases)
- Cart count observable
- `localStorage` persistence (cart survives across service instances)

---

## Project Structure

```
Quest-Internship-project/
├── .vscode/
│   ├── launch.json          # VS Code debug configurations (Backend / Frontend / Both)
│   └── tasks.json           # Underlying build and serve tasks
├── backend/
│   ├── ECommerce.Api/
│   │   ├── Controllers/     # HTTP endpoints (Auth, Products, Checkout)
│   │   ├── Services/        # Business logic
│   │   ├── Repositories/    # Data access (raw SQL via SqlClient)
│   │   ├── Models/          # Domain entities
│   │   ├── DTOs/            # Request / response shapes
│   │   └── appsettings.json # Connection string and JWT config
│   └── ECommerce.Tests/     # xUnit unit tests
├── database/
│   └── init.sql             # Creates DB, tables, and seeds products
├── frontend/
│   └── ecommerce-client/
│       ├── public/
│       │   └── images/      # Product images served as static assets
│       └── src/app/
│           ├── components/  # Angular page and UI components
│           ├── services/    # CartService (localStorage), ProductService, etc.
│           └── models/      # TypeScript interfaces
└── README.md
```
