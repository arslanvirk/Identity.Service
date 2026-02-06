# Project Guide (Compact)

A short overview of the Identity Service for quick reference.

---

## Overview
- ASP.NET Core API on .NET 9
- PostgreSQL database
- Swagger + Health checks
- JWT authentication

---

## Run Locally

```bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/identity/swagger`
- Health: `http://localhost:8080/health`

---

## Configuration

Required environment variables:
- `ConnectionStrings__DefaultConnection`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__SigningKey`

---

## Database
- Migrations run automatically on startup.

---

## Security Notes
- Never commit `.env` or secrets.
- Use strong passwords and rotate secrets regularly.

---

## CI/CD
- See `docs/CI-CD-SETUP.md` for the step-by-step pipeline guide.
