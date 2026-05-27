# Architecture

## Overview

Bifrost is a REST API built with **ASP.NET Core (.NET 10)** following **Hexagonal Architecture** (Ports & Adapters). The core domain is fully isolated from infrastructure and delivery concerns — all external dependencies (database, HTTP) are behind interfaces that the core defines.

**Stack:** ASP.NET Core 10 · Entity Framework Core 10 · PostgreSQL (Npgsql) · OpenAPI/Swagger

---

## Layer Structure

```
Bifrost/
├── Controller/               # Delivery layer — HTTP adapters
│   ├── Request/              # Inbound DTOs (validation attributes)
│   └── Response/             # Outbound DTOs
│
├── Core/                     # Domain — no external dependencies
│   ├── Domain/               # Domain models and DTOs (pure C# records/classes)
│   ├── Service/              # Use-case implementations
│   ├── Adapter/              # Service interfaces (I*Service) — input ports
│   ├── Port/Repository/      # Repository interfaces (I*Repository) — output ports
│   └── Exception/            # Domain exceptions (extend CoreException)
│
├── Infrastructure/
│   └── Persistence/
│       ├── ApplicationDbContext
│       ├── Entity/           # EF Core entity models
│       └── Repository/       # Repository implementations
│
├── Config/                   # Middleware extensions (exception handler)
├── Migrations/               # EF Core migrations
└── Program.cs                # Bootstrap and DI wiring
```

---

## Request Flow

```
HTTP Request
    │
    ▼
Controller          (maps Request DTO → domain DTO, calls service)
    │
    ▼
I*Service           (input port — interface defined in Core/Adapter)
    │
    ▼
*Service            (use-case logic, domain validation, throws domain exceptions)
    │
    ▼
I*Repository        (output port — interface defined in Core/Port/Repository)
    │
    ▼
RepositoryBase      (generic EF Core implementation — maps domain ↔ entity)
    │
    ▼
ApplicationDbContext → PostgreSQL
```

---

## Key Abstractions

### `IRepository<T>` — Generic repository port

Defined in `Core/Port/Repository/IRepository.cs`. Provides the baseline CRUD contract:

```csharp
GetAll()  /  Add(T)  /  Update(T)  /  FindById(Guid)  /  IdExists(Guid)  /  DeleteById(Guid)
```

Domain-specific repositories extend this interface to add aggregate-specific queries (e.g., `ICourseRepository.ExistsByCode(string)`).

### `RepositoryBase<TE, TD>` — Generic repository implementation

Abstract base in `Infrastructure/Persistence/Repository/RepositoryBase.cs`. Concrete repositories extend it and implement two abstract methods:

```csharp
protected abstract TD EntityToDomain(TE entity);
protected abstract TE DomainToEntity(TD domain);
```

This keeps EF entity models out of the core and forces an explicit mapping boundary.

### `CoreException` — Domain exception base

All business rule violations extend `CoreException`, which carries an HTTP `StatusCode`. The global exception handler middleware (`Config/HandlerExceptionConfig.cs`) intercepts exceptions and maps them to ProblemDetails responses:

| Exception type | HTTP status |
|---|---|
| `CoreException` subclass | Value of `StatusCode` property |
| `ArgumentException` | 400 |
| `KeyNotFoundException` | 404 |
| Anything else | 500 (message hidden, error logged) |

---

## Dependency Injection Lifetimes

| Type | Lifetime | Reason |
|---|---|---|
| `ApplicationDbContext` | Scoped | One context per HTTP request |
| `I*Service` | Scoped | Tied to request lifetime |
| `I*Repository` | Transient | Stateless; resolved per use |

---

## Data Model Separation

Each aggregate has four distinct representations:

| Layer | Type | Purpose |
|---|---|---|
| Controller | `*BodyRequest` / `*Response` | HTTP contract with validation |
| Core | `*Dto` | Carrying input data into a use case |
| Core | Domain class (e.g., `Course`) | Business model operated on by services |
| Infrastructure | `*Entity` | EF Core mapping to database table |

Mappings are performed at each boundary; no layer leaks its own type into another.

---

## Adding a New Aggregate

1. **Core/Domain** — create domain class and DTO record.
2. **Core/Exception** — create `NotFoundException` and any business-rule exceptions extending `CoreException`.
3. **Core/Port/Repository** — create `I*Repository` extending `IRepository<Domain>`.
4. **Core/Adapter** — create `I*Service` with the use-case signatures.
5. **Core/Service** — implement the service using the repository port.
6. **Infrastructure/Persistence/Entity** — create `*Entity` extending `EntityBase`.
7. **Infrastructure/Persistence/Repository** — extend `RepositoryBase<Entity, Domain>`, implement `EntityToDomain` / `DomainToEntity`, add aggregate-specific queries.
8. **ApplicationDbContext** — add `DbSet<*Entity>`.
9. **Controller** — create controller with Request/Response DTOs.
10. **Program.cs** — register repository (`AddTransient`) and service (`AddScoped`).

---

## Running the Project

### Prerequisites

- .NET 10 SDK
- PostgreSQL running on `localhost:5432`
- `dotnet-ef` local tool (already configured in `dotnet-tools.json`)

### Database connection

The default connection string is set in `Bifrost/appsettings.json`:

```
Server=localhost;Port=5432;Database=bifrost;User id=postgres;Password=postgres
```

Override it in `Bifrost/appsettings.Development.json` or via environment variable `ConnectionStrings__DefaultConnection` for a local setup without touching tracked files.

### Start the API

```bash
# Restore tools (first time only)
dotnet tool restore

# From the solution root
dotnet run --project Bifrost
```

The API will be available at:

| Profile | URL |
|---|---|
| HTTP | `http://localhost:5018` |
| HTTPS | `https://localhost:7118` |

Swagger UI (Development only): `http://localhost:5018/swagger`

---

## Migrations

`dotnet-ef` is installed as a local tool (version 10.0.5). All commands must be run from the solution root or the `Bifrost/` project directory.

```bash
# Restore tools (first time only)
dotnet tool restore

# Apply all pending migrations to the database
dotnet ef database update --project Bifrost

# Create a new migration after changing entities
dotnet ef migrations add <MigrationName> --project Bifrost

# Revert the last applied migration
dotnet ef database update <PreviousMigrationName> --project Bifrost

# Remove the last unapplied migration
dotnet ef migrations remove --project Bifrost
```

---

## Running Tests

The test project (`Bifrost.Test`) uses **xUnit** with **NSubstitute** for mocking and **FluentAssertions** for assertions.

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run a specific test class
dotnet test --filter "FullyQualifiedName~CourseServiceTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test project dependencies

| Package | Purpose |
|---|---|
| xUnit | Test framework |
| NSubstitute | Mocking/substituting interfaces |
| FluentAssertions | Expressive assertion syntax |
| coverlet | Code coverage collection |
