# Restaurants API

A production-ready ASP.NET Core Web API reference implementation showcasing Clean Architecture, CQRS, and modern .NET practices.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Status](https://img.shields.io/badge/status-in%20development-yellow)](https://github.com/TheMannerMan/Restaurants_WebApi)

## Overview

This project serves as a **reference implementation** and **architectural template** for building enterprise-grade Web APIs with ASP.NET Core. It demonstrates practical application of:

- Clean Architecture with clear layer separation
- CQRS pattern via MediatR
- Resource-based authorization with custom requirements
- Comprehensive validation and error handling
- Structured logging and performance monitoring
- Full test coverage with unit and integration tests

**Target Audience:** Developers building maintainable, scalable Web APIs who need a reference for architectural patterns and best practices.

> **Note:** This project is under active development. Additional tests, deployment preparation, and CI/CD pipelines are being added.

## Quick Start

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB (included with Visual Studio) or SQL Server instance

### Run Locally

```bash
# Clone the repository
git clone https://github.com/TheMannerMan/Restaurants_WebApi.git
cd Restaurants_WebApi

# Update connection string in Restaurants.API/appsettings.json if needed
# Default uses LocalDB: Server=(localdb)\\mssqllocaldb

# Run the API
cd Restaurants.API
dotnet run

# Navigate to API documentation
# https://localhost:5001/scalar/v1
```

Database migrations and seed data are applied automatically on startup.

## Architecture

### Layer Structure

```
┌─────────────────────────────────────────────────┐
│  Restaurants.API (Presentation)                 │
│  • Controllers                                  │
│  • Middleware (error handling, logging)         │
│  • OpenAPI configuration                        │
└─────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────┐
│  Restaurants.Application (Use Cases)            │
│  • Commands & Queries (CQRS)                    │
│  • DTOs & Mapping Profiles                      │
│  • Validators                                   │
│  • User Context                                 │
└─────────────────────────────────────────────────┘
                       ↓
┌─────────────────────────────────────────────────┐
│  Restaurants.Domain (Business Logic)            │
│  • Entities                                     │
│  • Repository Interfaces                        │
│  • Domain Exceptions                            │
│  • Business Rules                               │
└─────────────────────────────────────────────────┘
                       ↑
┌─────────────────────────────────────────────────┐
│  Restaurants.Infrastructure (External Concerns) │
│  • EF Core DbContext                            │
│  • Repository Implementations                   │
│  • Authorization Handlers                       │
│  • Identity Configuration                       │
└─────────────────────────────────────────────────┘
```

**Dependency Flow:** All dependencies point inward toward the Domain layer (Dependency Inversion Principle).

### Key Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| **CQRS with MediatR** | Separates read/write concerns, reduces controller complexity, enables pipeline behaviors |
| **Repository Pattern** | Abstracts data access, enables testing, provides flexibility for future data source changes |
| **FluentValidation** | Declarative validation with complex rules, automatic integration with API pipeline |
| **Resource-based Authorization** | Fine-grained access control beyond simple role checks (ownership, custom requirements) |
| **Structured Logging (Serilog)** | Queryable logs, performance monitoring, production diagnostics |

## Core Features

### Authentication & Authorization

- **JWT Bearer Authentication** via ASP.NET Core Identity
- **Role-based Authorization** (`User`, `Owner`, `Admin`)
- **Policy-based Authorization** with custom requirements:
  - `HasNationality` - Claim-based access control
  - `AtLeast20` - Age verification using custom `IAuthorizationRequirement`
  - `CreatedAtleast2Restaurants` - Resource-count based requirements
- **Resource-based Authorization** - Ownership verification for update/delete operations

```csharp
// Example: Custom authorization requirement
public class MinimumAgeRequirementsHandler : AuthorizationHandler<MinimumAgeRequirements>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumAgeRequirements requirement)
    {
        // Custom authorization logic with access to user claims
    }
}
```

### CQRS Implementation

Commands and Queries are handled via MediatR with automatic validation:

```csharp
// Command example
public record CreateRestaurantCommand : IRequest<int>
{
    public string Name { get; init; }
    public string Category { get; init; }
    // ...
}

public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, int>
{
    // Handler implementation
}

// Validation is automatic via FluentValidation
public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name).Length(3, 100);
        RuleFor(x => x.Category).Must(BeValidCategory);
    }
}
```

### Error Handling

Centralized error handling via middleware:

- `NotFoundException` → 404
- `ForbidException` → 403
- `ValidationException` → 400 (automatic via FluentValidation)
- Unhandled exceptions → 500 (logged with full context)

### Performance Monitoring

- **Request timing middleware** logs requests exceeding 4000ms
- **Serilog request logging** captures all HTTP requests
- **EF Core query logging** (configurable per environment)

## API Endpoints

### Identity (`/api/identity`)

- `POST /register` - Create new user account
- `POST /login` - Authenticate and receive JWT
- `POST /refresh` - Refresh access token

### Restaurants (`/api/restaurants`)

- `GET /` - List restaurants (pagination, filtering)
- `GET /{id}` - Get restaurant by ID
- `POST /` - Create restaurant (requires `Owner` role)
- `PATCH /{id}` - Update restaurant (requires ownership or `Admin`)
- `DELETE /{id}` - Delete restaurant (requires ownership or `Admin`)

### Dishes (`/api/restaurants/{restaurantId}/dishes`)

- `GET /` - List dishes for restaurant
- `GET /{dishId}` - Get dish by ID
- `POST /` - Create dish
- `DELETE /{dishId}` - Delete dish
- `DELETE /` - Delete all dishes for restaurant

**API Documentation:** Available at `/scalar/v1` in development mode.

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Framework** | .NET 10.0, ASP.NET Core Web API |
| **Architecture** | Clean Architecture, CQRS (MediatR) |
| **Data Access** | Entity Framework Core 10, SQL Server |
| **Authentication** | ASP.NET Core Identity, JWT Bearer |
| **Validation** | FluentValidation with auto-validation |
| **Mapping** | AutoMapper |
| **Logging** | Serilog with file and console sinks |
| **API Docs** | OpenAPI 3.0, Scalar UI |
| **Testing** | xUnit, FluentAssertions, Moq |

## Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Restaurants.Application.Tests
```

**Current Test Coverage:**
- ✅ Unit tests for command/query handlers
- ✅ Validator tests with positive and negative cases
- ✅ Authorization handler tests with mocked dependencies
- ✅ AutoMapper profile tests
- 🚧 Additional test coverage in progress

## Configuration

Key configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "RestaurantsDb": "Server=(localdb)\mssqllocaldb;Database=RestaurantsDb;Trusted_Connection=True;"
  },
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/Restaurant-API-.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## Development Principles

This codebase follows:

- **SOLID principles** - Especially Dependency Inversion and Single Responsibility
- **Clean Code** - Meaningful names, small functions, clear intent
- **DRY (Don't Repeat Yourself)** - Shared logic in base classes and utilities
- **KISS (Keep It Simple)** - Complexity only when justified
- **YAGNI (You Aren't Gonna Need It)** - No speculative features

## Extending This Project

This reference implementation focuses on core architectural patterns. Common extensions include:

- Caching strategies (Redis, in-memory)
- Rate limiting and throttling
- Health checks and readiness probes
- Containerization (Docker)
- API versioning strategies
- Distributed tracing

Refer to the [Issues](https://github.com/TheMannerMan/Restaurants_WebApi/issues) section for discussions on potential enhancements.

## Contributing

This is a personal reference project, but feedback and suggestions are welcome via Issues.

## Author

[TheMannerMan](https://github.com/TheMannerMan)

---

**Why this project exists:** To provide a practical, up-to-date reference implementation of Clean Architecture in .NET that can be referenced when building production applications or used as a starting point for new projects.
