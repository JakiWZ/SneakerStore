# SneakerStore

[![CI](https://github.com/JakiWZ/SneakerStore/actions/workflows/ci.yml/badge.svg)](https://github.com/JakiWZ/SneakerStore/actions/workflows/ci.yml)

A full-stack e-commerce API for sneakers, built with ASP.NET Core and Entity Framework Core. Supports user authentication, product browsing, a shopping cart, and a favorites list, backed by a role-based authorization system (Admin/User).

## Features

- **JWT authentication** with BCrypt password hashing and role-based authorization (`Admin` / `User`)
- **Product catalog** (CRUD) — public browsing, admin-only mutations
- **Shopping cart** — add/update/remove items, automatic line-total and cart-total calculation
- **Favorites list** per user
- **SQLite** persistence via EF Core, auto-seeded with sample data on first run
- **Secrets kept out of source control** — JWT signing key and admin seed credentials are read from [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) in development, never committed
- **Global exception handling** middleware — unhandled errors return a clean JSON response instead of leaking stack traces
- **Unit tests** (xUnit + EF Core InMemory) covering authentication and cart business logic

## Tech stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 10 (Minimal hosting model) |
| ORM | Entity Framework Core 10 (SQLite provider) |
| Auth | JWT Bearer + BCrypt.Net |
| Frontend | Vanilla HTML/CSS/JS (served as static files from `wwwroot`) |
| Tests | xUnit, EF Core InMemory provider |

## Architecture

The API follows a layered architecture to keep concerns separated and testable:

```
Controller  →  Service (business logic)  →  EF Core DbContext  →  SQLite
                    ↑
              Repository (used for the simpler Sneaker CRUD)
```

- **Controllers** handle HTTP concerns only (routing, status codes, model binding).
- **Services** (`IAuthService`, `ICartService`, `IFavoriteService`) hold business rules — e.g. password policy, stock checks, cart quantity accumulation.
- **DTOs** decouple the public API contract from the EF Core entities.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Configure secrets

The app expects a JWT signing key and (optionally) an admin account to seed on first run. Set them via `dotnet user-secrets` — **never commit these to source control**:

```bash
cd SneakerStore.Api
dotnet user-secrets set "Jwt:Key" "a-long-random-signing-key-at-least-32-chars"
dotnet user-secrets set "AdminSeed:Username" "admin"
dotnet user-secrets set "AdminSeed:Password" "a-strong-password"
```

### Run

```bash
cd SneakerStore.Api
dotnet run
```

The API and static frontend are served together; open the printed `localhost` URL in your browser. On first run, the database is created and seeded with sample sneakers, plus an Admin account using the `AdminSeed` credentials you configured above.

### Account roles

- `login.html` → "Register" creates a `User` account (browsing, cart, favorites only).
- Only an existing Admin can create another Admin: log in as Admin, then use the "Create Admin account" form (visible only to Admins). It calls `POST /api/Auth/create-admin`, which requires a valid Admin JWT — public registration can never grant the Admin role.

### Run the tests

```bash
cd SneakerStore.Tests
dotnet test
```

## API overview

| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/Auth/register` | POST | Public | Create a `User` account |
| `/api/Auth/login` | POST | Public | Returns a JWT |
| `/api/Auth/create-admin` | POST | Admin | Create another admin account |
| `/api/Sneaker` | GET | Public | List all sneakers |
| `/api/Sneaker/{id}` | GET | Public | Get one sneaker |
| `/api/Sneaker` | POST/PUT/DELETE | Admin | Manage catalog |
| `/api/Cart` | GET/POST/PUT/DELETE | User | Manage the current user's cart |
| `/api/Favorite` | GET/POST/DELETE | User | Manage the current user's favorites |

## Possible next steps

- Integration tests against the full HTTP pipeline (`WebApplicationFactory`)
- Pagination and filtering on the catalog endpoint
- Order/checkout flow
