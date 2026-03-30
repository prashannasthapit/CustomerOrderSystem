# Customer Order System API

ASP.NET Core Web API for managing Customers, Orders, and Order Items with EF Core Code First and PostgreSQL.

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core 9
- PostgreSQL

## Project Structure
- `CustomerOrderSystem/Models` - Domain entities
- `CustomerOrderSystem/DTOs` - Request/response contracts
- `CustomerOrderSystem/Data` - `AppDbContext` + Fluent API configuration
- `CustomerOrderSystem/Services` - Business logic layer
- `CustomerOrderSystem/Controllers` - REST endpoints
- `CustomerOrderSystem/Middleware` - Global exception handling
- `CustomerOrderSystem/Migrations` - EF Core migrations

## Prerequisites
- .NET SDK 10+
- PostgreSQL running locally (default `localhost:5432`)

## Configuration
Connection strings are in:
- `CustomerOrderSystem/appsettings.json`
- `CustomerOrderSystem/appsettings.Development.json`

Default development connection string:

`Host=localhost;Port=5432;Database=customer_order_system_dev;Username=postgres;Password=postgres`

## Run
```bash
cd CustomerOrderSystem
dotnet build
dotnet ef database update
dotnet run --urls http://localhost:5099
```

## Migration Commands
```bash
cd CustomerOrderSystem
dotnet ef migrations add InitialCreate --output-dir Migrations
dotnet ef database update
```

## API Endpoints
### Customers
- `GET /api/customers`
- `GET /api/customers/{id}`
- `POST /api/customers`
- `PATCH /api/customers/{id}`
- `DELETE /api/customers/{id}`

### Orders
- `GET /api/orders`
- `GET /api/orders/{id}`
- `GET /api/orders/customer/{customerId}`
- `POST /api/orders`
- `PATCH /api/orders/{id}`
- `DELETE /api/orders/{id}`

### OrderItems
- `GET /api/orderitems`
- `GET /api/orderitems/{id}`
- `GET /api/orderitems/order/{orderId}`
- `POST /api/orderitems`
- `PATCH /api/orderitems/{id}`
- `DELETE /api/orderitems/{id}`

## Notes
- Referential integrity is enforced with foreign keys.
- Customer deletion is blocked when orders exist.
- Order deletion is blocked when order items exist.
- Input validation uses DataAnnotations.
- API uses consistent JSON error responses via middleware.

