# Deployment Guide

## Overview
This guide covers deploying the Identity Service multi-container application using images from GitHub Container Registry (GHCR).

## Architecture
- **API Service**: ASP.NET Core 9.0 Web API
- **Database**: PostgreSQL 16
- **Admin Tool**: pgAdmin 4

All images are built via GitHub Actions and stored in GHCR.

---

## CI/CD Pipeline

### GitHub Actions Workflow
The CI pipeline builds and pushes three container images:

1. **API Service**: `ghcr.io/arslanvirk/identity.service:latest`
2. **Database**: `ghcr.io/arslanvirk/identity.service.db:latest`
3. **pgAdmin**: `ghcr.io/arslanvirk/identity.service.pgadmin:latest`

Each image is tagged with:
- `latest` - Always points to the most recent build
- `<git-sha>` - Specific commit SHA for rollback capability

### Triggering CI
```bash
# Automatic: Push to main branch
git push origin main

# Manual: Via GitHub UI
Actions ? CI/CD Pipeline ? Run workflow
```

---

## Local Deployment

### Prerequisites
- Docker Desktop or Docker Engine
- Docker Compose V2

### Steps

1. **Create environment file**
   ```bash
   cp .env.template .env
   # Edit .env with your values
   ```

2. **Login to GHCR**
   ```bash
   echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin
   ```

3. **Pull and run**
   ```bash
   docker compose -f docker-compose.prod.yml pull
   docker compose -f docker-compose.prod.yml up -d
   ```

4. **Verify deployment**
   ```bash
   docker compose -f docker-compose.prod.yml ps
   curl http://localhost:8080/health
   ```

5. **Access services**
   - API: http://localhost:8080
   - pgAdmin: http://localhost:5050

---

## Azure Deployment (Coming Soon)

The next phase will include:
- Azure Container Instances (ACI) deployment
- Azure Container Apps deployment
- Azure Key Vault integration for secrets
- Azure PostgreSQL flexible server option
- GitHub Actions CD workflow

### Prerequisites for Azure CD
- Azure subscription
- Azure CLI
- Service Principal with appropriate permissions

### Planned Azure Resources
```
Resource Group: rg-identity-service-prod
??? Container Registry (optional): acr-identity-service
??? Container Instance/App: identity-service-api
??? PostgreSQL Server: psql-identity-service
??? Key Vault: kv-identity-service
```

---

## Image Management

### View Available Images
```bash
# List all tags
docker images ghcr.io/arslanvirk/identity.service --format "table {{.Repository}}:{{.Tag}}\t{{.Size}}\t{{.CreatedAt}}"
```

### Use Specific Version
```bash
# Pull specific commit version
docker pull ghcr.io/arslanvirk/identity.service:a1b2c3d

# Update docker-compose.prod.yml
# Change: image: ghcr.io/arslanvirk/identity.service:latest
# To:     image: ghcr.io/arslanvirk/identity.service:a1b2c3d
```

### Rollback
```bash
# Stop current deployment
docker compose -f docker-compose.prod.yml down

# Update to previous SHA
# Edit docker-compose.prod.yml with older tag

# Redeploy
docker compose -f docker-compose.prod.yml up -d
```

---

## Security Best Practices

### Environment Variables
- ? Never commit `.env` files
- ? Use Azure Key Vault in production
- ? Rotate credentials regularly
- ? Use strong passwords (min 16 chars)

### JWT Configuration
```bash
# Generate secure signing key
openssl rand -base64 32
```

### Database Security
- Use strong passwords
- Restrict network access (Azure: use VNet integration)
- Enable SSL/TLS for connections
- Regular backups

### Container Security
- Images are scanned by GitHub Advanced Security
- Keep base images updated
- Run containers as non-root users
- Use read-only file systems where possible

---

## Monitoring & Logs

### View Logs
```bash
# All services
docker compose -f docker-compose.prod.yml logs -f

# Specific service
docker compose -f docker-compose.prod.yml logs -f api
```

### Health Checks
```bash
# API health endpoint
curl http://localhost:8080/health

# Database health
docker compose -f docker-compose.prod.yml exec db pg_isready -U postgres
```

---

## Troubleshooting

### Images Won't Pull
```bash
# Ensure you're logged in to GHCR
docker login ghcr.io

# Verify image exists
docker manifest inspect ghcr.io/arslanvirk/identity.service:latest
```

### API Can't Connect to Database
1. Check connection string in `.env`
2. Verify database is healthy: `docker compose ps`
3. Check network connectivity: `docker network inspect identity-service-prod_identity-net`

### Port Already in Use
```bash
# Find process using port
# Windows PowerShell:
Get-NetTCPConnection -LocalPort 8080

# Linux/Mac:
lsof -i :8080

# Change port in docker-compose.prod.yml
ports:
  - "8081:8080"  # Use 8081 on host instead
```

---

## Maintenance

### Backup Database
```bash
docker compose -f docker-compose.prod.yml exec db \
  pg_dump -U postgres identitydb > backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restore Database
```bash
cat backup_20240101_120000.sql | \
  docker compose -f docker-compose.prod.yml exec -T db \
  psql -U postgres identitydb
```

### Update to Latest Images
```bash
docker compose -f docker-compose.prod.yml pull
docker compose -f docker-compose.prod.yml up -d
```

### Clean Up Old Images
```bash
docker image prune -a
```

---

## Support

- **Repository**: https://github.com/arslanvirk/Identity.Service
- **Issues**: https://github.com/arslanvirk/Identity.Service/issues
- **Packages**: https://github.com/arslanvirk?tab=packages
