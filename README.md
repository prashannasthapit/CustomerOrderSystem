# Customer Order System API

ASP.NET Core Web API for managing Customers, Orders, and Order Items with EF Core Code First and PostgreSQL.

## Tech Stack
- ASP.NET Core Web API (.NET 10)
- Entity Framework Core 9
- PostgreSQL

## Project Structure
- `CustomerOrderSystem/` - API (Presentation): controllers, middleware, Program, Swagger
- `CustomerOrderSystem.Domain/` - Domain entities and repository/UoW contracts
- `CustomerOrderSystem.Application/` - DTOs, service interfaces/implementations, business exceptions
- `CustomerOrderSystem.Infrastructure/` - EF Core `AppDbContext`, migrations, repository implementations, transaction/UoW, DI registration

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
dotnet build CustomerOrderSystem.slnx
dotnet ef database update --project CustomerOrderSystem.Infrastructure --startup-project CustomerOrderSystem
dotnet run --project CustomerOrderSystem --urls http://localhost:5099
```

## Migration Commands
```bash
dotnet ef migrations add InitialCreate --project CustomerOrderSystem.Infrastructure --startup-project CustomerOrderSystem --output-dir Migrations
dotnet ef database update --project CustomerOrderSystem.Infrastructure --startup-project CustomerOrderSystem
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
- Repositories use a shared generic `RepositoryBase<T>` for common CRUD/query operations.
- Writes are transaction-safe via Unit of Work and explicit EF Core transactions.
- Input validation uses DataAnnotations.
- API uses consistent JSON error responses via middleware.

