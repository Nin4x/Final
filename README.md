# Loan API

This repository provides a small loan management API built with ASP.NET Core using a layered/Clean Architecture structure. The solution is split into:

- **LoanApi.Domain** – entity and enum definitions for users, loans, and roles.【F:LoanApi/LoanApi.Domain/Entities/User.cs†L6-L25】【F:LoanApi/LoanApi.Domain/Entities/Loan.cs†L5-L20】【F:LoanApi/LoanApi.Domain/Enums/UserRole.cs†L3-L7】
- **LoanApi.Application** – business services, DTOs, validation, and application-level dependency injection.【F:LoanApi/LoanApi.Application/DependencyInjection.cs†L11-L19】
- **LoanApi.Infrastructure** – EF Core persistence, JWT generation, repositories, and design-time factory setup.【F:LoanApi/LoanApi.Infrastructure/DependencyInjection.cs†L17-L39】【F:LoanApi/LoanApi.Infrastructure/Persistence/AppDbContextFactory.cs†L6-L13】
- **LoanApi.Api** – the ASP.NET Core host, middleware, authentication/authorization setup, and controllers.【F:LoanApi/LoanApi.Api/Program.cs†L6-L38】【F:LoanApi/LoanApi.Api/Configurations/ServiceCollectionExtensions.cs†L17-L89】【F:LoanApi/LoanApi.Api/Controllers/AuthController.cs†L19-L37】【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L21-L74】【F:LoanApi/LoanApi.Api/Controllers/AccountantController.cs†L23-L44】

## Setup

1. **Connection string** – Update `ConnectionStrings:LoanDatabase` in `LoanApi.Api/appsettings.json` (default is SQLite file `loanapi.db`). If the connection string is empty, the API falls back to an in-memory database, which means data will be lost on restart.【F:LoanApi/LoanApi.Api/appsettings.json†L2-L56】【F:LoanApi/LoanApi.Infrastructure/DependencyInjection.cs†L17-L28】
2. **Design-time factory** – EF Core tooling uses `AppDbContextFactory`, which targets SQLite by default for migrations.【F:LoanApi/LoanApi.Infrastructure/Persistence/AppDbContextFactory.cs†L6-L13】
3. **Seeded accountant** – On startup an accountant account is seeded from the `AccountantUser` section (username/email/password). Change these values if needed before first run.【F:LoanApi/LoanApi.Api/appsettings.json†L52-L56】【F:LoanApi/LoanApi.Infrastructure/Persistence/DatabaseSeeder.cs†L16-L58】

### Migrations

The project uses EF Core with Sqlite; migrations are optional if you rely on in-memory storage. To create or update a SQLite database:

```bash
dotnet tool install --global dotnet-ef   # if not already installed
dotnet ef migrations add InitialCreate -s LoanApi/LoanApi.Api -p LoanApi/LoanApi.Infrastructure
dotnet ef database update -s LoanApi/LoanApi.Api -p LoanApi/LoanApi.Infrastructure
```

> When you change the connection string to a persistent store, run `database update` again to create the schema.

## How to Run

```bash
cd LoanApi
# restore and run the API
DOTNET_ENVIRONMENT=Development dotnet run --project LoanApi.Api
```

The app starts with Serilog logging and Swagger UI enabled in Development. Controllers are mapped under `/api/*`, and an accountant user is seeded before requests are processed.【F:LoanApi/LoanApi.Api/Program.cs†L16-L36】【F:LoanApi/LoanApi.Infrastructure/Persistence/DatabaseSeeder.cs†L16-L58】

## How to Test

```bash
cd LoanApi
dotnet test LoanApi.sln
```

Integration tests cover auth, loan rules, and accountant privileges.

## JWT Usage

- JWT settings (issuer, audience, secret, expiry) live under `Jwt` in `appsettings.json`. Tokens expire after `ExpiryMinutes` (60 by default).【F:LoanApi/LoanApi.Api/appsettings.json†L39-L44】
- Authentication is configured with the Bearer scheme; validation checks issuer, audience, lifetime, and signing key with zero clock skew.【F:LoanApi/LoanApi.Api/Configurations/ServiceCollectionExtensions.cs†L30-L51】
- Successful login/register responses return an `AccessToken` and `ExpiresOnUtc` to include in the `Authorization: Bearer <token>` header.【F:LoanApi/LoanApi.Api/Controllers/AuthController.cs†L19-L37】【F:LoanApi/LoanApi.Application/DTOs/AuthResponse.cs†L3-L6】

## Roles and Endpoints

- **Anonymous** – `POST /api/auth/register`, `POST /api/auth/login` for creating users and obtaining JWTs.【F:LoanApi/LoanApi.Api/Controllers/AuthController.cs†L19-L37】
- **User** – default role for self-service loan management. Requires JWT with `Role=User`.
  - `GET /api/loans` and `GET /api/loans/{id}` list/read only the caller’s loans unless caller is an accountant.【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L21-L40】
  - `POST /api/loans` creates a loan; only available to `User` role.【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L42-L52】
  - `PUT /api/loans/{id}` updates an existing loan (ownership enforced in the service layer).【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L54-L63】
  - `DELETE /api/loans/{id}` deletes a loan the caller is allowed to manage.【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L65-L74】
- **Accountant** – elevated role seeded on startup. Requires JWT with `Role=Accountant`.
  - Can view all loans via the same `/api/loans` and `/api/loans/{id}` endpoints.【F:LoanApi/LoanApi.Api/Controllers/LoansController.cs†L21-L40】
  - `PATCH /api/accountant/users/{id}/block` toggles user block status.【F:LoanApi/LoanApi.Api/Controllers/AccountantController.cs†L23-L33】
  - `PATCH /api/accountant/loans/{id}/status` updates loan status (e.g., approve/reject).【F:LoanApi/LoanApi.Api/Controllers/AccountantController.cs†L35-L44】

## Example cURL requests

Register a user and capture the returned token:

```bash
token=$(curl -s -X POST http://localhost:5000/api/auth/register \
  -H 'Content-Type: application/json' \
  -d '{
    "username": "jane",
    "email": "jane@example.com",
    "password": "Pass123!",
    "age": 30,
    "monthlyIncome": 5000,
    "role": 0
  }' | jq -r '.accessToken')
```

Create a loan as a user:

```bash
curl -X POST http://localhost:5000/api/loans \
  -H "Authorization: Bearer $token" \
  -H 'Content-Type: application/json' \
  -d '{
    "borrowerName": "Jane Doe",
    "amount": 10000,
    "currency": 0,
    "periodMonths": 12,
    "type": 0,
    "interestRate": 5.5
  }'
```

Get loans (user sees own; accountant sees all):

```bash
curl -H "Authorization: Bearer $token" http://localhost:5000/api/loans
```

Accountant-only actions (use the seeded accountant credentials to log in first):

```bash
accountant_token=<token-from-accountant-login>
# Block or unblock a user
curl -X PATCH http://localhost:5000/api/accountant/users/<userId>/block \
  -H "Authorization: Bearer $accountant_token" \
  -H 'Content-Type: application/json' \
  -d '{ "isBlocked": true }'

# Update loan status
curl -X PATCH http://localhost:5000/api/accountant/loans/<loanId>/status \
  -H "Authorization: Bearer $accountant_token" \
  -H 'Content-Type: application/json' \
  -d '{ "status": 1 }'
```
