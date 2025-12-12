# LoanApi

LoanApi is a production-ready ASP.NET Core Web API for managing user loans with JWT authentication and role-based authorization.

## Architecture

The solution follows a layered architecture:

- **LoanApi.Domain** – Entities and enums.
- **LoanApi.Application** – DTOs, validators, service interfaces, business services, mapping profiles.
- **LoanApi.Infrastructure** – EF Core data access, repositories, JWT and password hashing helpers, Serilog logging, seeding.
- **LoanApi.Api** – Controllers, authentication setup, middleware, Swagger.
- **LoanApi.Tests** – Unit tests for business rules.

## Authentication & Authorization

- JWT Bearer authentication with claims for user id, username, role, and block status.
- Roles: `User`, `Accountant`.
- Blocked users cannot create new loans.

## Entities

- **User**: Id, FirstName, LastName, Username, Age, Email, MonthlyIncome, IsBlocked, PasswordHash, Role.
- **Loan**: Id, LoanType (FastLoan, AutoLoan, Installment), Amount, Currency, Period, Status (Processing, Approved, Rejected), UserId.

## Key Business Rules

- Users can view only their own profile/loans and modify loans only while status is `Processing`.
- Accountants can view/update/delete any loan and change status to `Approved` or `Rejected`.
- Accountants can block users (`IsBlocked = true`).

## Running the API

1. Ensure .NET 8 SDK and SQLite are available.
2. Restore dependencies and build:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project LoanApi.Api/LoanApi.Api.csproj
   ```
3. The API uses SQLite by default (`loanapi.db`). Update `appsettings.json` if needed.
4. Database will migrate and seed an accountant user on startup (credentials from `Seed` section in `appsettings.json`).

## Testing

Run unit tests:

```bash
dotnet test
```

## Swagger

Swagger UI is available at `/swagger` in development for exploring endpoints.

## Sample Endpoints

- `POST /api/auth/register` – Register user.
- `POST /api/auth/login` – Obtain JWT.
- `GET /api/users/{id}` – Get own profile (or any profile for accountants).
- `POST /api/loans` – Create loan (non-blocked users).
- `GET /api/loans` – List own loans or all loans (accountant).
- `PATCH /api/accountant/users/{id}/block` – Block a user.
- `PATCH /api/accountant/loans/{id}/status?status=approved` – Change loan status.

Use the returned JWT in the `Authorization: Bearer <token>` header for authorized endpoints.
