# Configuration

The application reads configuration from `appsettings.*.json`, environment variables, and user-secrets (dev). In containers, environment variables are the primary mechanism.

## Required Settings

- Connection string:
  - `ConnectionStrings:DefaultConnection`
  - Environment variable form: `ConnectionStrings__DefaultConnection`
  - Example: `Host=db;Port=5432;Database=identitydb;Username=postgres;Password=postgrespw;Pooling=true`

- JWT:
  - `Jwt:Issuer` → `Jwt__Issuer`
  - `Jwt:Audience` → `Jwt__Audience`
  - Signing key (recommended for stable tokens): `Jwt:SigningKey` → `Jwt__SigningKey`
    - Note: Current code generates a random key per startup; prefer configuring a stable key for non-dev.

- Environment:
  - `ASPNETCORE_ENVIRONMENT` (e.g., Development, Staging, Production)
  - Custom: `EnvironmentName` (optional)

## Example (Docker Compose)

## Visual Studio User-Secrets (Dev)
- Right-click project → Manage User Secrets
- Example `secrets.json`:

## Notes
- Never commit real secrets. Prefer environment variables or secret stores.
- For production, rotate credentials and configure HTTPS, CORS, and headers appropriately.
