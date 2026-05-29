# Bifrost

Bifrost is a REST API that supports the academic evaluation lifecycle of a university. It manages the core entities involved in an assessment season — courses, disciplines, academic centers, coordinations, and their members — and handles authentication via Google OAuth 2.0.

Access control follows a two-tier model: any registered user can read data, while write operations (create, update, delete) are restricted to administrators.

## Documentation

| Document | Description |
|---|---|
| [Architecture](docs/ARCHITECTURE.md) | Layer structure, key abstractions, request flow, and guide for adding new aggregates |
| [Domain Model](docs/domain-model.md) | Class diagram of the core domain entities and their relationships |
| [Authentication & Access Control](docs/access-control.md) | Google OAuth 2.0 flow, authorization policies, and per-route access matrix |

## Quick start

**Prerequisites:** .NET 10 SDK · PostgreSQL on `localhost:5432`

```bash
# 1. Restore local tools
dotnet tool restore

# 2. Apply database migrations
dotnet ef database update --project Bifrost

# 3. Run the API
dotnet run --project Bifrost
```

API available at `http://localhost:5018` · Swagger UI at `http://localhost:5018/swagger`

## Authentication

Bifrost delegates identity to Google. To authenticate:

1. Obtain a Google authorization code (scopes: `openid email`)
2. Exchange it for tokens via `POST /auth/token`
3. Use the returned `access_token` as a Bearer token on all subsequent requests

See [Authentication & Access Control](docs/access-control.md) for the full flow.

## Running tests

```bash
dotnet test
```

## Tech stack

- ASP.NET Core 10
- Entity Framework Core 10 + Npgsql (PostgreSQL)
- Google OAuth 2.0
- xUnit · NSubstitute · FluentAssertions
