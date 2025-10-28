# Overview

Identity.Service is an ASP.NET Core Identity–based REST API built on .NET 9. It provides user management (registration, login, password reset), JWT issuance, and health/observability. Data is stored in PostgreSQL via Entity Framework Core with a dedicated database schema.

## Key Features
- User registration and authentication with ASP.NET Core Identity
- JWT bearer tokens (configurable issuer/audience)
- Password reset flow using invite links
- Health checks and Swagger/OpenAPI
- Structured request/response logging with sensitive-field masking
- Docker Compose for local orchestration (API + PostgreSQL + optional pgAdmin)

## Endpoints
- Swagger UI: GET `/identity/swagger` (doc: `/identity/swagger/v1/swagger.json`)
- Health: GET `/identity/identity/health`
- API base path convention: `identity/identity/api/v1` (via controller base path registration)

## Solution Structure
- `Identity.Service.Web` (Presentation/API): ASP.NET Core pipeline, DI, Swagger, health checks, middlewares
- `Identity.Service.Application` (Application): DTOs, services (e.g., `AuthService`), constants, exceptions
- `Identity.Service.Infrastructure` (Data): EF Core context (`EFDataContext`), repositories, migrations, seed
- `Identity.Service.Core` (Domain): Entities and repository interfaces

## Tech Stack
- .NET 9, ASP.NET Core Identity
- EF Core 9 + Npgsql provider (PostgreSQL)
- Swashbuckle (Swagger), HealthChecks
- Docker + Docker Compose

## Quickstart
- Docker:
  - `docker compose up -d --build`
  - API: http://localhost:8080
  - Swagger: http://localhost:8080/identity/swagger
  - Health: http://localhost:8080/identity/identity/health
- Visual Studio 2022:
  - Set `docker-compose` as Startup Project → F5

## Database Schema
All Identity and domain tables are created under schema `identity_base`. Migrations are applied automatically on startup.