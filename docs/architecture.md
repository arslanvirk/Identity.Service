# Architecture

The solution follows a layered architecture with clear separation of concerns. The Web layer handles HTTP concerns and delegates to Application services, which orchestrate domain operations via Infrastructure repositories.

## Layers
- Web (Presentation):
  - ASP.NET Core pipeline, DI composition, middlewares (error handling, auth, logging)
  - Swagger/OpenAPI and health checks
- Application:
  - Services encapsulating use cases (e.g., `AuthService`)
  - DTOs, validation, exceptions, constants
- Infrastructure:
  - EF Core `EFDataContext` (schema: `identity_base`)
  - Repositories, migrations, Npgsql provider
- Core (Domain):
  - Entities (`User`, `UserRole`, `InviteUser`, `Configuration`)
  - Repository interfaces

## High-level Component Diagram

## Cross-cutting Concerns
- Authentication/Authorization:
  - ASP.NET Core Identity + JWT Bearer
  - Cookies enabled for certain flows (sliding expiration)
- Observability:
  - Health checks endpoint
  - Request/response logging with sensitive-field masking (passwords)
- Configuration:
  - AppSettings, environment variables, user-secrets (dev)
  - Docker Compose env vars for local orchestration

## Data Model Highlights
- Identity tables (roles, users, claims, logins, tokens) under `identity_base`
- Domain tables:
  - `users` (mapped entity fields and identity columns)
  - `invite_user` (password reset and related flows)
  - `configuration` (key/value app configuration)
