# Quick Reference Card

## ?? Start Services

**PowerShell:**
```powershell
.\start.ps1
```

**Bash:**
```bash
./start.sh
```

**Manual:**
```bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

---

## ?? Service URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| API | http://localhost:8080 | - |
| Swagger | http://localhost:8080/identity/swagger | - |
| Health | http://localhost:8080/health | - |
| Keycloak | http://localhost:8180 | admin / admin |
| pgAdmin | http://localhost:5050 | dev@local.test / admin123 |

---

## ?? Common Commands

### Docker Compose
```bash
# Start services
docker compose up -d

# Stop services
docker compose down

# Stop and remove volumes
docker compose down -v

# View logs (all services)
docker compose logs -f

# View logs (specific service)
docker compose logs -f keycloak

# Restart a service
docker compose restart keycloak

# Rebuild and start
docker compose up -d --build

# Check status
docker compose ps
```

### Keycloak
```bash
# Access container
docker exec -it identity-service-keycloak bash

# Check health
curl http://localhost:8180/health/ready

# View metrics
curl http://localhost:8180/metrics
```

### PostgreSQL
```bash
# Access database
docker exec -it identity-service-db psql -U postgres

# List databases
docker exec -it identity-service-db psql -U postgres -c "\l"

# Connect to Keycloak DB
docker exec -it identity-service-db psql -U postgres -d keycloakdb
```

---

## ?? Important Files

| File | Purpose |
|------|---------|
| `.env` | Local environment variables (DO NOT COMMIT) |
| `docker-compose.yml` | Service orchestration |
| `docs/KEYCLOAK-SETUP.md` | Detailed setup guide |
| `docs/KEYCLOAK-INTEGRATION-SUMMARY.md` | Integration summary |
| `docs/KEYCLOAK-CHECKLIST.md` | Step-by-step checklist |
| `docs/PROJECT-GUIDE.md` | Project overview |
| `docs/CI-CD-SETUP.md` | Pipeline documentation |

---

## ?? Environment Variables

```env
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb

# PostgreSQL
POSTGRES_PASSWORD=DevPassword123
POSTGRES_DB=identitydb
POSTGRES_USER=postgres

# JWT
JWT_ISSUER=http://identity.local
JWT_AUDIENCE=identity-clients
JWT_SIGNING_KEY=dev-signing-key-change-me
```

---

## ?? Troubleshooting

### Container won't start
```bash
docker compose logs [service-name]
docker compose ps
```

### Port conflicts
Check if ports are in use:
```powershell
# Windows
netstat -ano | findstr "8080"
netstat -ano | findstr "8180"

# Linux/Mac
lsof -i :8080
lsof -i :8180
```

### Database issues
```bash
# Check DB is running
docker compose ps db

# View DB logs
docker compose logs db

# Connect to DB
docker exec -it identity-service-db psql -U postgres
```

### Reset everything
```bash
docker compose down -v
docker compose up -d --build
```

---

## ?? CI/CD

**Trigger**: Push to `main` branch

**Images Published to GHCR**:
- `ghcr.io/arslanvirk/identity.service:latest`
- `ghcr.io/arslanvirk/identity.service.db:latest`
- `ghcr.io/arslanvirk/identity.service.pgadmin:latest`
- `ghcr.io/arslanvirk/identity.service.keycloak:latest`

**View Pipeline**: https://github.com/arslanvirk/Identity.Service/actions

---

## ?? Next Steps

1. ? Services running
2. ? Configure Keycloak realm
3. ? Create client in Keycloak
4. ? Add NuGet packages
5. ? Configure ASP.NET Core
6. ? Test authentication

See `docs/KEYCLOAK-CHECKLIST.md` for detailed steps.

---

## ?? Quick Links

- **GitHub Repo**: https://github.com/arslanvirk/Identity.Service
- **Keycloak Docs**: https://www.keycloak.org/documentation
- **Docker Docs**: https://docs.docker.com/compose/
- **ASP.NET Core**: https://learn.microsoft.com/aspnet/core/

---

## ?? Pro Tips

- Use `docker compose logs -f` to tail logs in real-time
- Press `Ctrl+C` to stop following logs (doesn't stop containers)
- Use `docker compose down -v` to completely reset (removes data)
- Keycloak takes ~30-60 seconds to start (wait for health check)
- PostgreSQL creates Keycloak DB automatically on first run
- Change default passwords before deploying to production!

---

**Print this card and keep it handy!** ??
