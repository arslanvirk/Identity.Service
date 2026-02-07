# Project Guide (Compact)

A short overview of the Identity Service for quick reference.

---

## Overview
- ASP.NET Core API on .NET 9
- PostgreSQL database
- Keycloak for identity and access management
- Swagger + Health checks
- JWT authentication

---

## Run Locally

```bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

### Service URLs

- **API**: `http://localhost:8080`
- **Swagger**: `http://localhost:8080/identity/swagger`
- **Health**: `http://localhost:8080/health`
- **Keycloak**: `http://localhost:8180` (admin/admin)
- **pgAdmin**: `http://localhost:5050`

---

## Configuration

Required environment variables:
- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__SigningKey`
- `KEYCLOAK_ADMIN`
- `KEYCLOAK_ADMIN_PASSWORD`

---

## Database
- Migrations run automatically on startup.
- PostgreSQL hosts both application DB (`identitydb`) and Keycloak DB (`keycloakdb`)

---

## Security Notes
- Never commit `.env` or secrets.
- Use strong passwords and rotate secrets regularly.
- Keycloak runs in development mode (disable HTTPS, embedded admin)

---

## CI/CD
- See `docs/CI-CD-SETUP.md` for the step-by-step pipeline guide.
