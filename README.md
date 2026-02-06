# Identity.Service

ASP.NET Core Identity-based REST API on .NET 9 using EF Core + PostgreSQL. Features registration, login (JWT), password reset, health checks, and Swagger.

## Quickstart

- Docker Compose (local dev):
  - `COMPOSE_PROFILES=dev docker compose up -d --build`
  - API: `http://localhost:8080`
  - Swagger: `http://localhost:8080/identity/swagger`
  - Health: `http://localhost:8080/health`

## Configuration (env vars)

- `ConnectionStrings__DefaultConnection=Host=db;Port=5432;Database=identitydb;Username=postgres;Password=postgrespw;Pooling=true`
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SigningKey`
- Note: For production, supply a stable JWT signing key and do not use default passwords.

## Documentation (Compact)

- `docs/PROJECT-GUIDE.md`
- `docs/CI-CD-SETUP.md`
- `DEPLOYMENT.md`

## License

MIT (or your chosen license)
