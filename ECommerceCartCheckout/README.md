# E-Commerce Cart & Checkout API

This project is a simplified full-stack e-commerce vertical slice built with Angular, ASP.NET Core Web API, and SQL Server.

## Technologies Used

- ASP.NET Core Web API (.NET 10.0)
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

---

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET SDK 10.0** or later ([Download](https://dotnet.microsoft.com/download))
- **Node.js 18+** and npm ([Download](https://nodejs.org))
- **SQL Server 2019+** or **SQL Server Express** ([Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))
- **SQL Server Management Studio (SSMS)** or another SQL management tool (optional but recommended)
- **Angular CLI** (will be installed locally)

---

## Project Setup

### Step 1: Clone the Repository

```bash
git clone <your-repo-url>
cd ECommerceCartCheckout
```

### Step 2: Database Setup

#### 2.1 Restore the Database Backup

1. Open **SQL Server Management Studio (SSMS)** or connect to your SQL Server instance
2. Right-click on **Databases** → **Restore Database**
3. Select **Device** and browse to: `database/ECommerceDb.bak.sql`
4. Click **OK** to restore the database

Alternatively, using PowerShell:
```powershell
# Restore the database backup
sqlcmd -S <your-server-name> -U sa -P <your-password> -i "database\ECommerceDb.bak.sql"
```

Or using the SQL script directly:
```powershell
sqlcmd -S <your-server-name> -U sa -P <your-password> -i "database\init.sql"
```

#### 2.2 Verify the Database

Connect to SQL Server and verify that the `ECommerceDb` database exists with tables:
- `Users`
- `Products`
- `Orders`
- `OrderItems`

---

## Backend Setup (.NET Web API)

### Step 1: Navigate to Backend Directory

```bash
cd backend/ECommerce.Api
```

### Step 2: Update Connection String

Edit `appsettings.Development.json` and update the SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<your-server>;Database=ECommerceDb;User Id=sa;Password=<your-password>;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers"
  }
}
```

Replace:
- `<your-server>` with your SQL Server instance (e.g., `localhost` or `(localdb)\mssqllocaldb`)
- `<your-password>` with your SQL Server password
- `Jwt:Key` with a secure secret key (minimum 32 characters)

### Step 3: Restore Dependencies

```bash
dotnet restore
```

### Step 4: Run the Backend Server

```bash
dotnet run
```

The API will start at: `http://localhost:5000` (or the port specified in `launchSettings.json`)

### Step 5: Verify the Backend

Open Swagger UI in your browser:
```
http://localhost:5000/swagger/index.html
```

---

## Frontend Setup (Angular)

### Step 1: Navigate to Frontend Directory

```bash
cd frontend/ecommerce-client
```

### Step 2: Install Dependencies

```bash
npm install
```

### Step 3: Configure API Base URL

Edit `src/app/services/auth.service.ts`, `product.service.ts`, `cart.service.ts`, and `checkout.service.ts`:

Ensure the API base URL is set correctly:
```typescript
private apiUrl = 'http://localhost:5000/api';
```

### Step 4: Start the Development Server

```bash
ng serve
```

The frontend will start at: `http://localhost:4200`

### Step 5: Verify the Frontend

Open your browser and navigate to:
```
http://localhost:4200
```

---

## Running the Complete Application

1. **Start SQL Server** (ensure it's running)
2. **Start the Backend** in one terminal:
   ```bash
   cd backend/ECommerce.Api
   dotnet run
   ```
3. **Start the Frontend** in another terminal:
   ```bash
   cd frontend/ecommerce-client
   ng serve
   ```
4. **Open your browser** to `http://localhost:4200`

---

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login user and receive JWT token

### Products
- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID

### Checkout
- `POST /api/checkout` - Place an order

All endpoints (except register and login) may require JWT authentication.

---

## Running Unit Tests

### Backend Tests (xUnit)

```bash
cd backend/ECommerce.Tests
dotnet test
```

### Frontend Tests (Jasmine/Karma)

```bash
cd frontend/ecommerce-client
ng test
```

Or run tests once without watching:
```bash
ng test --watch=false
```

---

## Project Structure

```
ECommerceCartCheckout/
├── backend/
│   ├── ECommerce.Api/
│   │   ├── Controllers/        # API endpoints
│   │   ├── Services/           # Business logic
│   │   ├── Repositories/       # Data access layer
│   │   ├── Models/             # Domain models
│   │   ├── DTOs/               # Data transfer objects
│   │   └── Program.cs          # Startup configuration
│   └── ECommerce.Tests/        # Unit tests
├── frontend/
│   └── ecommerce-client/
│       ├── src/app/
│       │   ├── components/     # Angular components
│       │   ├── services/       # Angular services
│       │   └── models/         # TypeScript interfaces
│       └── package.json
├── database/
│   ├── ECommerceDb.bak.sql     # Database backup
│   └── init.sql                # Database initialization script
└── README.md
```

---


## Database Setup

The database is named:

```text
ECommerceDb