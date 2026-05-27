# Bifrost

Bifrost is a REST API for evaluating different sectors of a university. Students, professors, and coordinators can submit evaluations for courses, academic centers, disciplines, and classrooms according to their role and enrollment.

## Evaluated sectors

| Sector | Who can evaluate |
|---|---|
| **Coordination** (course coordination) | Any student or professor |
| **Academic Center** | Any student or professor |
| **Discipline** | Only students enrolled in the discipline |
| **Classroom** | Only professors who taught a discipline in that classroom |

## Documentation

| Document | Description |
|---|---|
| [Architecture](docs/ARCHITECTURE.md) | Architectural overview, layer structure, key abstractions, and step-by-step guide for adding new aggregates |

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

## Running tests

```bash
dotnet test
```

## Tech stack

- ASP.NET Core 10
- Entity Framework Core 10 + Npgsql (PostgreSQL)
- xUnit · NSubstitute · FluentAssertions
