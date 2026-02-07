# Deployment Guide (Simple)

This guide shows the basic local run steps using `docker-compose.yml`.

---

## Local (Development)

### Option A — `.env` file (recommended)

1. Create the `.env` file in the repo root (already provided for local dev).

2. Start local dev:
 
PowerShell:
```$env:COMPOSE_PROFILES="dev"
docker compose up -d --build
```

CMD:
```
set COMPOSE_PROFILES=dev
docker compose up -d --build
```

bash:
```
COMPOSE_PROFILES=dev docker compose up -d --build
```

### Service URLs

Once running, you can access:

- **API**: http://localhost:8080
- **API Swagger**: http://localhost:8080/identity/swagger
- **Keycloak Admin Console**: http://localhost:8180
  - Username: `admin`
  - Password: `admin`
- **pgAdmin**: http://localhost:5050
  - Email: `dev@local.test`
  - Password: `admin123`

### Keycloak Setup

Keycloak runs in development mode with:
- PostgreSQL backend (shared with API, separate database `keycloakdb`)
- Admin credentials from `.env` file
- Health checks enabled at `/health/ready`
- Metrics enabled for monitoring

**Note**: Keycloak runs on port **8180** (not 8080) to avoid conflict with the API.

---

## CI/CD

For the beginner step‑by‑step CI/CD pipeline setup, see:
- `docs/CI-CD-SETUP.md`
