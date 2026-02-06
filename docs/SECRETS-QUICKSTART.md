# Secrets Management - Quick Start

Choose your deployment method:

## ?? Local Development (Recommended)

**Use `.env` file for simplicity:**

```bash
# 1. Create .env
cp .env.template .env

# 2. Edit with your values
notepad .env  # Windows
nano .env     # Linux/Mac

# 3. Run
docker compose -f docker-compose.prod.yml up -d
```

**Access:**
- API: http://localhost:8080
- pgAdmin: http://localhost:5050

---

## ?? Production (Docker Secrets)

**Use Docker Secrets for maximum security:**

### Windows:
```powershell
# 1. Run setup
.\scripts\setup-secrets.ps1

# 2. Deploy
docker compose -f docker-compose.prod.secrets.yml up -d
```

### Linux/Mac:
```bash
# 1. Run setup
chmod +x scripts/setup-secrets.sh
./scripts/setup-secrets.sh

# 2. Deploy
docker compose -f docker-compose.prod.secrets.yml up -d
```

---

## ?? Comparison

| Feature | `.env` (Local) | Docker Secrets (Production) |
|---------|----------------|----------------------------|
| **Setup Time** | 1 minute | 3 minutes |
| **Security** | Good | Excellent |
| **Ease of Use** | Very Easy | Easy |
| **Recommended For** | Dev, Testing | Staging, Production |
| **Secrets Location** | `.env` file | `secrets/*.txt` files |
| **Compose File** | `docker-compose.prod.yml` | `docker-compose.prod.secrets.yml` |

---

## ?? Verify Deployment

```bash
# Check running containers
docker ps

# Test API health
curl http://localhost:8080/health

# View logs
docker logs identity-service-api

# For Docker Secrets: Verify secrets mounted
docker exec identity-service-api ls -la /run/secrets/
```

---

## ?? Full Documentation

See [SECRETS-MANAGEMENT.md](./SECRETS-MANAGEMENT.md) for:
- Detailed setup instructions
- Security best practices
- Troubleshooting guide
- Migration between methods

---

## ?? Important

- ? `.env` and `secrets/` are gitignored
- ? Never commit secrets to version control
- ? Use strong passwords (16+ characters)
- ? Rotate secrets every 60-90 days

---

**Quick Help:**
```bash
# Stop all containers
docker compose -f docker-compose.prod.yml down
# or
docker compose -f docker-compose.prod.secrets.yml down

# Remove all data (including volumes)
docker compose -f docker-compose.prod.yml down -v
```
