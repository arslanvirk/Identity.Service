# Secrets Management Guide

This guide explains how to manage secrets and environment variables for the Identity Service in both **local development** and **production** environments.

---

## ?? Overview

| Environment | Method | Files | Use Case |
|-------------|--------|-------|----------|
| **Local/Dev** | `.env` file | `.env` + `docker-compose.prod.yml` | Development, testing, quick setup |
| **Production** | Docker Secrets | `secrets/*.txt` + `docker-compose.prod.secrets.yml` | Production, staging, secure deployment |

---

## ?? Table of Contents

1. [Local Development Setup (.env)](#1-local-development-setup-env)
2. [Production Setup (Docker Secrets)](#2-production-setup-docker-secrets)
3. [Security Best Practices](#3-security-best-practices)
4. [Troubleshooting](#4-troubleshooting)
5. [Migration Between Methods](#5-migration-between-methods)

---

## 1?? Local Development Setup (.env)

### When to Use
- ? Local development on your machine
- ? Quick testing and experimentation
- ? Team members with individual configurations
- ? CI/CD pipelines (with encrypted secrets)

### Setup Steps

#### Step 1: Create `.env` File
```bash
# Copy the template
cp .env.template .env
```

#### Step 2: Edit `.env` with Your Values
```bash
# Windows
notepad .env

# Linux/Mac
nano .env
```

#### Step 3: Fill in Configuration
```env
# JWT Configuration
JWT_ISSUER=https://localhost:8080
JWT_AUDIENCE=local-dev
JWT_SIGNING_KEY=kJ8v3Yx9Qw2Ln5Pm7Rt8Uv6Wz4Ac1Bd3Ef5Gh7Ij9Kl==

# Database Configuration
POSTGRES_PASSWORD=DevPassword123!
POSTGRES_DB=identitydb
POSTGRES_USER=postgres
CONNECTION_STRING=Host=db;Port=5432;Database=identitydb;Username=postgres;Password=DevPassword123!;Pooling=true

# pgAdmin Configuration
PGADMIN_EMAIL=dev@local.test
PGADMIN_PASSWORD=DevAdmin123!
```

#### Step 4: Run with Docker Compose
```bash
# Start services
docker compose -f docker-compose.prod.yml up -d

# View logs
docker compose -f docker-compose.prod.yml logs -f

# Stop services
docker compose -f docker-compose.prod.yml down
```

### How It Works
```yaml
# docker-compose.prod.yml reads ${VARIABLE} from .env
services:
  api:
    environment:
      Jwt__Issuer: ${JWT_ISSUER}  # ? Value from .env
```

### Advantages
- ? Simple and fast setup
- ? Easy to edit and update
- ? Works with most IDEs
- ? Standard Docker Compose pattern

### Disadvantages
- ?? File can be accidentally committed (if not gitignored)
- ?? Less secure than Docker Secrets
- ?? Not recommended for production

---

## 2?? Production Setup (Docker Secrets)

### When to Use
- ? Production environments
- ? Staging environments
- ? Docker Swarm deployments
- ? Maximum security requirements
- ? Kubernetes (via sealed secrets)

### Setup Steps

#### Step 1: Run Secrets Setup Script

**Windows (PowerShell):**
```powershell
# Navigate to project root
cd "D:\Documents\Mine\Apps\Asp.net Identity\Identity.Service"

# Run setup script
.\scripts\setup-secrets.ps1
```

**Linux/Mac (Bash):**
```bash
# Navigate to project root
cd ~/projects/Identity.Service

# Make script executable
chmod +x scripts/setup-secrets.sh

# Run setup script
./scripts/setup-secrets.sh
```

#### Step 2: Follow Interactive Prompts

The script will ask for:
```
?? JWT Configuration
===================
?? Enter JWT Issuer URL:
   (Press Enter for default: https://api.identityservice.com)
[Hidden input]

?? Enter JWT Audience:
   (Press Enter for default: identity-clients)
[Hidden input]

?? JWT Signing Key
   Generate a secure key? (y/n)
y
? Generated and saved JWT signing key

???  Database Configuration
========================
?? Enter PostgreSQL Password:
[Hidden input]

?? pgAdmin Configuration
=======================
?? Enter pgAdmin Email:
   (Press Enter for default: admin@example.com)
[Hidden input]

?? Enter pgAdmin Password:
[Hidden input]
```

#### Step 3: Verify Secrets Created

```bash
# Check secrets directory
ls -la secrets/

# Output:
# -rw------- 1 user group  32 jwt_issuer.txt
# -rw------- 1 user group  16 jwt_audience.txt
# -rw------- 1 user group  44 jwt_signing_key.txt
# -rw------- 1 user group 120 connection_string.txt
# -rw------- 1 user group  20 postgres_password.txt
# -rw------- 1 user group  25 pgadmin_email.txt
# -rw------- 1 user group  18 pgadmin_password.txt
```

#### Step 4: Deploy with Docker Secrets

```bash
# Start services with secrets
docker compose -f docker-compose.prod.secrets.yml up -d

# Verify deployment
curl http://localhost:8080/health

# Check logs
docker compose -f docker-compose.prod.secrets.yml logs -f api

# Check secrets are mounted
docker exec identity-service-api ls -la /run/secrets/
```

### How It Works

**1. Secrets stored in files:**
```
secrets/
??? jwt_issuer.txt
??? jwt_audience.txt
??? jwt_signing_key.txt
??? connection_string.txt
??? postgres_password.txt
??? pgadmin_email.txt
??? pgadmin_password.txt
```

**2. Docker Compose mounts secrets:**
```yaml
services:
  api:
    secrets:
      - jwt_signing_key  # Mounted to /run/secrets/jwt_signing_key

secrets:
  jwt_signing_key:
    file: ./secrets/jwt_signing_key.txt  # Source file
```

**3. Application reads from mounted files:**
```csharp
// In your ASP.NET Core app (future enhancement)
var jwtKey = File.ReadAllText("/run/secrets/jwt_signing_key");
```

### Advantages
- ? **Maximum security** - Secrets never in environment variables
- ? **Encrypted at rest** - Docker encrypts secrets
- ? **Access control** - Only authorized containers access secrets
- ? **Audit trail** - Track who accessed what
- ? **Production-ready** - Industry best practice

### Disadvantages
- ?? More complex initial setup
- ?? Requires script or manual secret creation
- ?? Needs application code changes (to read from files)

---

## 3?? Security Best Practices

### For Both Methods

#### ? DO:
1. **Never commit secrets to Git**
   ```bash
   # Verify .gitignore
   cat .gitignore | grep -E "\.env|secrets"
   
   # Output should include:
   # .env
   # secrets/
   ```

2. **Use strong passwords**
   - Minimum 16 characters
   - Mix of uppercase, lowercase, numbers, symbols
   - No dictionary words

3. **Generate secure JWT keys**
   ```bash
   # Generate 32-byte base64 key
   openssl rand -base64 32
   
   # Windows PowerShell
   $bytes = New-Object byte[] 32
   [Security.Cryptography.RNGCryptoServiceProvider]::Create().GetBytes($bytes)
   [Convert]::ToBase64String($bytes)
   ```

4. **Rotate secrets regularly**
   - JWT keys: Every 90 days
   - Passwords: Every 60 days
   - After any security incident

5. **Backup secrets securely**
   ```bash
   # Encrypt backup
   tar -czf secrets-backup.tar.gz secrets/
   gpg -c secrets-backup.tar.gz
   rm secrets-backup.tar.gz
   
   # Store encrypted file in secure location
   ```

#### ? DON'T:
1. ? Use default passwords
2. ? Share `.env` or `secrets/` via email/chat
3. ? Commit secrets to Git (even private repos)
4. ? Use same secrets across environments
5. ? Store secrets in cloud storage without encryption

### Environment-Specific Recommendations

| Environment | Method | Additional Security |
|-------------|--------|-------------------|
| **Local Dev** | `.env` | Use weak passwords for convenience |
| **Testing** | `.env` | Use test data, not prod secrets |
| **Staging** | Docker Secrets | Mirror prod security, use different values |
| **Production** | Docker Secrets + Key Vault | Add Azure Key Vault, rotate automatically |

---

## 4?? Troubleshooting

### Problem: `.env` file not loaded

**Symptoms:**
```bash
docker logs identity-service-api
# Error: Configuration 'Jwt__Issuer' not found
```

**Solutions:**
```bash
# 1. Verify .env exists in same directory as docker-compose
ls -la .env

# 2. Check Docker Compose version (needs v2+)
docker compose version

# 3. Explicitly specify env file
docker compose -f docker-compose.prod.yml --env-file .env up -d

# 4. Check syntax in .env (no spaces around =)
# ? Wrong: JWT_ISSUER = value
# ? Correct: JWT_ISSUER=value
```

### Problem: Docker Secrets not found

**Symptoms:**
```bash
docker compose -f docker-compose.prod.secrets.yml up
# Error: secret "jwt_signing_key" references file "./secrets/jwt_signing_key.txt" which is not found
```

**Solutions:**
```bash
# 1. Verify secrets directory exists
ls -la secrets/

# 2. Run setup script
.\scripts\setup-secrets.ps1  # Windows
./scripts/setup-secrets.sh    # Linux/Mac

# 3. Check file permissions
chmod 600 secrets/*.txt  # Linux/Mac

# 4. Verify file paths in docker-compose.prod.secrets.yml
cat docker-compose.prod.secrets.yml | grep "file:"
```

### Problem: Secrets not readable by container

**Symptoms:**
```bash
docker exec identity-service-api cat /run/secrets/jwt_signing_key
# cat: /run/secrets/jwt_signing_key: No such file or directory
```

**Solutions:**
```bash
# 1. Check if secret is mounted
docker exec identity-service-api ls -la /run/secrets/

# 2. Verify secret is declared in docker-compose
docker compose -f docker-compose.prod.secrets.yml config | grep -A 5 "secrets:"

# 3. Restart container
docker compose -f docker-compose.prod.secrets.yml restart api
```

### Problem: PostgreSQL won't start with Docker Secrets

**Symptoms:**
```bash
docker logs identity-service-db
# Error: database "identitydb" does not exist
```

**Solution:**
The `docker-compose.prod.secrets.yml` uses a custom command to read password from file:
```yaml
command: >
  bash -c "export POSTGRES_PASSWORD=$$(cat /run/secrets/postgres_password) && 
  docker-entrypoint.sh postgres"
```

Verify the secret file exists:
```bash
cat secrets/postgres_password.txt
```

---

## 5?? Migration Between Methods

### From `.env` to Docker Secrets

```bash
# 1. Create secrets directory
mkdir -p secrets
chmod 700 secrets

# 2. Copy values from .env to individual files
cat .env | grep JWT_ISSUER | cut -d= -f2 > secrets/jwt_issuer.txt
cat .env | grep JWT_AUDIENCE | cut -d= -f2 > secrets/jwt_audience.txt
cat .env | grep JWT_SIGNING_KEY | cut -d= -f2 > secrets/jwt_signing_key.txt
cat .env | grep POSTGRES_PASSWORD | cut -d= -f2 > secrets/postgres_password.txt
cat .env | grep CONNECTION_STRING | cut -d= -f2 > secrets/connection_string.txt
cat .env | grep PGADMIN_EMAIL | cut -d= -f2 > secrets/pgadmin_email.txt
cat .env | grep PGADMIN_PASSWORD | cut -d= -f2 > secrets/pgadmin_password.txt

# 3. Set restrictive permissions
chmod 600 secrets/*.txt

# 4. Test with new compose file
docker compose -f docker-compose.prod.secrets.yml up -d

# 5. Verify and remove .env
docker compose -f docker-compose.prod.secrets.yml ps
rm .env
```

### From Docker Secrets to `.env` (for testing)

```bash
# 1. Create .env from template
cp .env.template .env

# 2. Manually copy values from secrets/ files
# Or use this script:
echo "JWT_ISSUER=$(cat secrets/jwt_issuer.txt)" >> .env
echo "JWT_AUDIENCE=$(cat secrets/jwt_audience.txt)" >> .env
echo "JWT_SIGNING_KEY=$(cat secrets/jwt_signing_key.txt)" >> .env
echo "POSTGRES_PASSWORD=$(cat secrets/postgres_password.txt)" >> .env
echo "CONNECTION_STRING=$(cat secrets/connection_string.txt)" >> .env
echo "PGADMIN_EMAIL=$(cat secrets/pgadmin_email.txt)" >> .env
echo "PGADMIN_PASSWORD=$(cat secrets/pgadmin_password.txt)" >> .env

# 3. Test
docker compose -f docker-compose.prod.yml up -d
```

---

## ?? Reference

### Files Overview

| File | Purpose | Tracked in Git |
|------|---------|----------------|
| `.env.template` | Template with placeholder values | ? Yes |
| `.env` | Actual secrets for local dev | ? No |
| `secrets/*.txt` | Production secrets (one per file) | ? No |
| `docker-compose.prod.yml` | Uses `.env` file | ? Yes |
| `docker-compose.prod.secrets.yml` | Uses Docker Secrets | ? Yes |
| `scripts/setup-secrets.sh` | Linux/Mac setup script | ? Yes |
| `scripts/setup-secrets.ps1` | Windows setup script | ? Yes |

### Environment Variable Mapping

| `.env` Variable | Docker Secret File | Container Mount Path | ASP.NET Config Key |
|-----------------|-------------------|----------------------|-------------------|
| `JWT_ISSUER` | `jwt_issuer.txt` | `/run/secrets/jwt_issuer` | `Jwt:Issuer` |
| `JWT_AUDIENCE` | `jwt_audience.txt` | `/run/secrets/jwt_audience` | `Jwt:Audience` |
| `JWT_SIGNING_KEY` | `jwt_signing_key.txt` | `/run/secrets/jwt_signing_key` | `Jwt:SigningKey` |
| `POSTGRES_PASSWORD` | `postgres_password.txt` | `/run/secrets/postgres_password` | N/A (DB only) |
| `CONNECTION_STRING` | `connection_string.txt` | `/run/secrets/connection_string` | `ConnectionStrings:DefaultConnection` |
| `PGADMIN_EMAIL` | `pgadmin_email.txt` | `/run/secrets/pgadmin_email` | N/A (pgAdmin only) |
| `PGADMIN_PASSWORD` | `pgadmin_password.txt` | `/run/secrets/pgadmin_password` | N/A (pgAdmin only) |

---

## ?? Quick Reference Commands

### Local Development (.env)
```bash
# Setup
cp .env.template .env && nano .env

# Run
docker compose -f docker-compose.prod.yml up -d

# Logs
docker compose -f docker-compose.prod.yml logs -f

# Stop
docker compose -f docker-compose.prod.yml down
```

### Production (Docker Secrets)
```bash
# Setup (Windows)
.\scripts\setup-secrets.ps1

# Setup (Linux/Mac)
chmod +x scripts/setup-secrets.sh && ./scripts/setup-secrets.sh

# Run
docker compose -f docker-compose.prod.secrets.yml up -d

# Verify secrets
docker exec identity-service-api ls -la /run/secrets/

# Logs
docker compose -f docker-compose.prod.secrets.yml logs -f

# Stop
docker compose -f docker-compose.prod.secrets.yml down
```

---

## ?? Additional Resources

- [Docker Secrets Documentation](https://docs.docker.com/engine/swarm/secrets/)
- [ASP.NET Core Configuration](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/)
- [Azure Key Vault Integration](https://learn.microsoft.com/en-us/azure/key-vault/)
- [OWASP Secrets Management](https://cheatsheetseries.owasp.org/cheatsheets/Secrets_Management_Cheat_Sheet.html)

---

**Last Updated:** 2024  
**Maintainer:** Identity Service Team
