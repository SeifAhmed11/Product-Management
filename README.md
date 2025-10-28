# Product Management (Clean Architecture, .NET 8 + Angular 16)

Backend: .NET 8 Web API with Clean Architecture (EF Core, DI, soft delete, unique SKU, pagination, static auth header).
Frontend: Angular 16 app (Material UI) for listing/searching/pagination, add/edit, soft delete, CSV export.

## Requirements
- .NET SDK 8
- Node 18+ and npm
- SQL Server LocalDB (or any SQL Server)

## Solution Structure
- ProductManagement.Domain — entities and domain logic
- ProductManagement.Application — DTOs, services, abstractions
- ProductManagement.Infrastructure — EF Core, DbContext, configurations, repositories, DI
- ProductManagement.API — controllers, filters, middlewares, Program
- ProductManagement.Web — Angular 16 frontend (Material)

## Backend (API)
Default base URL (dev): `https://localhost:5001` (Kestrel HTTPS)
Alternative HTTP used by the UI: `http://localhost:5000`

### Setup and Run
1) Restore and build:
```bash
dotnet build
```

2) Create initial migration (if needed):
```bash
dotnet ef migrations add InitialCreate --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

3) Apply database updates:
```bash
dotnet ef database update --project ProductManagement.Infrastructure --startup-project ProductManagement.API
```

Migrations also auto-apply at startup via `db.Database.Migrate()`.

4) Run API:
```bash
dotnet run --project ProductManagement.API
```

### Authentication Header
Send this header with every request:
- `X-Auth-Token: static_secret`

### Endpoints
- GET `/api/products?search=&page=1&pageSize=10&includeDeleted=false`
- GET `/api/products/{id}`
- POST `/api/products`
- PUT `/api/products/{id}`
- DELETE `/api/products/{id}` (soft delete)

### Request Examples
- Create:
```http
POST /api/products HTTP/1.1
Host: localhost:5000
Content-Type: application/json
X-Auth-Token: static_secret

{
  "name": "Sample Product",
  "sku": "SKU-001",
  "description": "Sample description",
  "price": 19.99
}
```

- Update:
```http
PUT /api/products/1 HTTP/1.1
Host: localhost:5000
Content-Type: application/json
X-Auth-Token: static_secret

{
  "name": "Updated Name",
  "sku": "SKU-001",
  "description": "Updated description",
  "price": 29.99
}
```

- Query:
```http
GET /api/products?page=1&pageSize=10 HTTP/1.1
Host: localhost:5000
X-Auth-Token: static_secret
```

- Soft Delete:
```http
DELETE /api/products/1 HTTP/1.1
Host: localhost:5000
X-Auth-Token: static_secret
```

### Notes
- Global query filter for soft delete is applied to `Product`. Reading deleted rows is possible internally by disabling filters; UI provides a toggle using `includeDeleted`.
- Unique `Sku` is enforced via DB unique index and service checks.
- CORS is enabled for `http://localhost:4200`, and preflight (OPTIONS) is allowed.

## Frontend (Angular)
Location: `ProductManagement.Web`

### Install and Run
```bash
cd ProductManagement.Web
npm install
ng serve --open
```

The app expects the API at `http://localhost:5000/api/products` and sends `X-Auth-Token: static_secret` via an HTTP interceptor.

### Features
- Products list with search (debounced), pagination, show deleted toggle
- Add/Edit product (reactive forms with validation)
- Soft delete
- Export visible list to CSV

If you want to change the API base URL or token, edit:
- `src/app/core/services/product.service.ts` (base URL)
- `src/app/core/interceptors/auth.interceptor.guard.ts` (header token)

