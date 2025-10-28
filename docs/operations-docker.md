# Operations with Docker

This project includes Docker Compose to run the API, PostgreSQL, and optional pgAdmin locally.

## Services
- `api`: Builds from `Identity.Service.Web/Dockerfile`, listens on port `8080` in-container
- `db`: `postgres:16`, initialized with `identitydb`
- `pgadmin` (optional): `dpage/pgadmin4:8` for DB admin UI (port `5050`)

## Run/Stop

## Endpoints
- API: http://localhost:8080
- Swagger: http://localhost:8080/identity/swagger
- Health: http://localhost:8080/identity/identity/health
- pgAdmin: http://localhost:5050

## Environment Variables (excerpt)

## pgAdmin Login and Server Registration
- Login: `admin@example.com` / `admin123!` (change in your env)
- Add new Server:
  - General → Name: `Identity Postgres` (any)
  - Connection:
    - Host: `db` (if pgAdmin is containerized) or `localhost` (if pgAdmin is on host)
    - Port: `5432`
    - Maintenance DB: `postgres` (or `identitydb`)
    - Username: `postgres`
    - Password: `postgrespw`

If credentials were changed after first pgAdmin start, recreate the container (pgAdmin applies env vars only on first run):

## Visual Studio 2022
- Include `docker-compose.dcproj` in the solution
- Set `docker-compose` as Startup Project → F5
- VS uses the provided Dockerfile for fast debug cycles

## Volumes and Persistence
- Postgres data persisted in `pgdata` volume
- Optionally persist pgAdmin config with a named volume (e.g., `pgadmin-data` → `/var/lib/pgadmin`)

## Troubleshooting
- No tables? Ensure the API started and applied migrations on startup.
- Connection refused: confirm ports are not in use and containers are healthy.
- JWT errors: verify `Jwt__Issuer`, `Jwt__Audience`, and provide a stable signing key for non-dev.