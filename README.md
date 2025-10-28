# Identity.Service

ASP.NET Core Identity-based REST API on .NET 9 using EF Core + PostgreSQL. Features registration, login (JWT), password reset, health checks, and Swagger.

## Quickstart

- Docker Compose:
  - docker compose up -d --build
  - API: http://localhost:8080
  - Swagger: http://localhost:8080/identity/swagger
  - Health: http://localhost:8080/identity/identity/health
- Visual Studio 2022:
  - Set `docker-compose` as Startup Project → F5

## Configuration (env vars)

- `ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=identitydb;Username=postgres;Password=postgrespw;Pooling=true`
- `Jwt__Issuer`, `Jwt__Audience`
- Note: For production, supply a stable JWT signing key and do not use default passwords.

## Architecture

- Web: ASP.NET Core API, middleware, Swagger, health.
- Application: DTOs, services (`AuthService`), validation.
- Infrastructure: EF Core (`EFDataContext`), repositories, migrations, Npgsql.
- Core: Entities, repository contracts.

## Tech Stack

- .NET 9, ASP.NET Core Identity, EF Core 9, Npgsql
- Swagger/Swashbuckle, HealthChecks
- Docker + Docker Compose

Key packages (Web)
- Microsoft.AspNetCore.Authentication.JwtBearer 9.0.8
- Microsoft.EntityFrameworkCore.Design 9.0.8
- Swashbuckle.AspNetCore 9.0.3

[Full package list → docs/overview.md]

## Database & Migrations

- Applies migrations on startup (`db.Database.MigrateAsync()`).
- Details and manual commands: see docs/database.md.

## Security

- Supply a stable JWT signing key via configuration in non-dev.
- Rotate default credentials; never commit secrets.

## Documentation

- docs/overview.md
- docs/architecture.md
- docs/configuration.md
- docs/operations-docker.md
- docs/database.md
- docs/security.md

## License

MIT (or your chosen license)
