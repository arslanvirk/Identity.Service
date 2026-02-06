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
---

## CI/CD

For the beginner step‑by‑step CI/CD pipeline setup, see:
- `docs/CI-CD-SETUP.md`
