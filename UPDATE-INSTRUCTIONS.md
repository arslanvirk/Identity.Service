# Keycloak Simplification - Update Instructions

This document provides exact changes to simplify the Keycloak configuration to use embedded H2 database.

---

## 1. Update `docker-compose.yml`

### Change 1: Simplify Keycloak Dev Service (Lines ~119-133)

**REMOVE:**
```yaml
  keycloak:
    profiles: ["dev"]
    image: quay.io/keycloak/keycloak:26.0.7
    container_name: identity-service-keycloak
    environment:
      KC_BOOTSTRAP_ADMIN_USERNAME: ${KEYCLOAK_ADMIN:-admin}
      KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-admin}
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://db:5432/${KEYCLOAK_DB:-keycloakdb}
      KC_DB_USERNAME: ${POSTGRES_USER:-postgres}
      KC_DB_PASSWORD: ${POSTGRES_PASSWORD:-DevPassword123}
      KC_HEALTH_ENABLED: "true"
      KC_METRICS_ENABLED: "true"
    command: start-dev
    depends_on:
      db:
        condition: service_healthy
    <<: *keycloak-common
```

**REPLACE WITH:**
```yaml
  keycloak:
    profiles: ["dev"]
    image: quay.io/keycloak/keycloak:26.5.2
    container_name: identity-service-keycloak
    environment:
      KC_BOOTSTRAP_ADMIN_USERNAME: ${KEYCLOAK_ADMIN:-admin}
      KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-admin}
    command: start-dev
    <<: *keycloak-common
```

### Change 2: Update Keycloak CI Service (Lines ~162-165)

**REMOVE:**
```yaml
  keycloak-ci:
    profiles: ["ci"]
    image: quay.io/keycloak/keycloak:26.0.7
    networks:
      - identity-net
```

**REPLACE WITH:**
```yaml
  keycloak-ci:
    profiles: ["ci"]
    image: quay.io/keycloak/keycloak:26.5.2
    networks:
      - identity-net
```

### Change 3: Remove Keycloak Volume (Lines ~168-174)

**REMOVE:**
```yaml
volumes:
  pgdata:
    driver: local
  pgadmin-data:
    driver: local
  keycloak-data:
    driver: local
```

**REPLACE WITH:**
```yaml
volumes:
  pgdata:
    driver: local
  pgadmin-data:
    driver: local
```

---

## 2. Update `.env`

**REMOVE:**
```env
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```

**REPLACE WITH:**
```env
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

---

## 3. Update `.github/workflows/ci.yml`

### Change 1: Update Image Pull (Line ~51)
**Find:**
```yaml
docker compose -f ${{ env.COMPOSE_FILE }} --profile ${{ env.COMPOSE_PROFILE }} pull db-ci pgadmin-ci keycloak-ci
```

**Keep as is** (no change needed)

### Change 2: Update Image Tag (Lines ~63-64)
**Find:**
```yaml
docker tag quay.io/keycloak/keycloak:26.0.7 ${{ env.KEYCLOAK_TAG_LATEST }}
docker tag quay.io/keycloak/keycloak:26.0.7 ${{ env.KEYCLOAK_TAG_SHA }}
```

**REPLACE WITH:**
```yaml
docker tag quay.io/keycloak/keycloak:26.5.2 ${{ env.KEYCLOAK_TAG_LATEST }}
docker tag quay.io/keycloak/keycloak:26.5.2 ${{ env.KEYCLOAK_TAG_SHA }}
```

---

## 4. Update `docs/KEYCLOAK-SETUP.md`

### Change: Update Architecture Section
**Find the "Database Configuration" section and UPDATE:**

**REMOVE:**
```markdown
### Database Configuration

Keycloak uses PostgreSQL as its backend database:
- **Database**: `keycloakdb` (separate from the API's `identitydb`)
- **Host**: `db` (shared PostgreSQL container)
- **Connection**: Configured via `KC_DB_*` environment variables
```

**REPLACE WITH:**
```markdown
### Database Configuration

Keycloak uses an embedded H2 database:
- **Database**: H2 (embedded, in-memory by default)
- **Persistence**: Data is lost when container restarts
- **Purpose**: Development and testing only
- **Note**: For production, use external PostgreSQL (see Phase 2)
```

### Update Development Mode Section
**Find and UPDATE:**

**REMOVE:**
```markdown
### Health Checks

Keycloak includes health checks at:
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

Metrics are enabled at `/metrics` for monitoring.
```

**REPLACE WITH:**
```markdown
### Health Checks

Keycloak includes health checks at `/health/ready` for container orchestration.
```

---

## 5. Update `docs/KEYCLOAK-INTEGRATION-SUMMARY.md`

### Update "What Was Added" Section

**Find and UPDATE the Keycloak Service section:**

**REMOVE:**
```markdown
### 1. Keycloak Service
- **Image**: `quay.io/keycloak/keycloak:26.0.7`
- **Port**: `8180` (to avoid conflict with API on 8080)
- **Admin Console**: http://localhost:8180
- **Default Credentials**: admin/admin (configurable via `.env`)
- **Database**: PostgreSQL (separate database `keycloakdb`)
- **Mode**: Development mode (`start-dev` command)
```

**REPLACE WITH:**
```markdown
### 1. Keycloak Service
- **Image**: `quay.io/keycloak/keycloak:26.5.2`
- **Port**: `8180` (to avoid conflict with API on 8080)
- **Admin Console**: http://localhost:8180
- **Default Credentials**: admin/admin (configurable via `.env`)
- **Database**: Embedded H2 (data lost on restart)
- **Mode**: Development mode (`start-dev` command)
```

### Update Environment Configuration Section

**REMOVE:**
```markdown
Updated `.env` file with:
```env
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```
```

**REPLACE WITH:**
```markdown
Updated `.env` file with:
```env
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```
```

---

## 6. Update `docs/QUICK-REFERENCE.md`

### Update Environment Variables Section

**Find and UPDATE:**

**REMOVE:**
```env
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```

**REPLACE WITH:**
```env
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

---

## 7. Update `docs/PROJECT-GUIDE.md`

### Update Database Section

**Find and UPDATE:**

**REMOVE:**
```markdown
## Database
- Migrations run automatically on startup.
- PostgreSQL hosts both application DB (`identitydb`) and Keycloak DB (`keycloakdb`)
```

**REPLACE WITH:**
```markdown
## Database
- Migrations run automatically on startup.
- PostgreSQL hosts application DB (`identitydb`)
- Keycloak uses embedded H2 database (data lost on restart)
```

---

## 8. Update `DEPLOYMENT.md`

### Update Keycloak Setup Section

**Find and UPDATE:**

**REMOVE:**
```markdown
### Keycloak Setup

Keycloak runs in development mode with:
- PostgreSQL backend (shared with API, separate database `keycloakdb`)
- Admin credentials from `.env` file
- Health checks enabled at `/health/ready`
- Metrics enabled for monitoring

**Note**: Keycloak runs on port **8180** (not 8080) to avoid conflict with the API.
```

**REPLACE WITH:**
```markdown
### Keycloak Setup

Keycloak runs in development mode with:
- Embedded H2 database (data lost on restart)
- Admin credentials from `.env` file
- Health checks enabled at `/health/ready`

**Note**: 
- Keycloak runs on port **8180** (not 8080) to avoid conflict with the API
- Data is NOT persistent - for persistent data, use PostgreSQL backend (see docs)
```

---

## 9. Update `docs/ARCHITECTURE-DIAGRAM.md`

### Update Database Schema Section

**Find and UPDATE:**

**REMOVE:**
```markdown
## Database Schema

PostgreSQL hosts two databases:

1. **identitydb** - API database
   - User accounts
   - Application data
   - Managed by EF Core migrations

2. **keycloakdb** - Keycloak database
   - Realms, clients, users
   - Identity configuration
   - Auto-created by Keycloak
```

**REPLACE WITH:**
```markdown
## Database Schema

### PostgreSQL
**identitydb** - API database
- User accounts
- Application data
- Managed by EF Core migrations

### Keycloak H2 Database (Embedded)
- Realms, clients, users
- Identity configuration
- In-memory (data lost on restart)
- For persistent data, configure PostgreSQL backend
```

---

## 10. Update `CHANGELOG-KEYCLOAK.md`

### Add Simplification Entry at Top

**ADD AT THE TOP:**
```markdown
# Keycloak Integration - Change Log

## [Simplified] - [Current Date]

### Changed
- Simplified Keycloak to use embedded H2 database (development mode)
- Removed PostgreSQL backend configuration
- Updated Keycloak version from 26.0.7 to 26.5.2
- Removed `KEYCLOAK_DB` environment variable
- Removed `keycloak-data` volume (not needed with H2)
- Removed database dependency from Keycloak service
- Removed health and metrics explicit configuration

### Rationale
- Simpler setup for development and testing
- Faster startup time
- Easier to understand for beginners
- Matches original simple Keycloak setup pattern
- Data persistence not required for local development

### Trade-offs
- ?? Data is lost when container restarts
- ?? Not suitable for production
- ? Simpler configuration
- ? No database setup required
- ? Good for learning and prototyping

---

[Previous changelog entries follow...]
```

---

## 11. Update `SESSION-SUMMARY.md`

### Update Current State Section

**Find the Keycloak Integration section and UPDATE:**

**REMOVE:**
```markdown
2. **Keycloak Integration** (JUST COMPLETED)
   - Keycloak 26.0.7 running on port 8180
   - PostgreSQL backend (separate database: keycloakdb)
   - Development mode configured
   - Admin console accessible at http://localhost:8180 (admin/admin)
   - Health checks and metrics enabled
   - Integrated with existing PostgreSQL instance
```

**REPLACE WITH:**
```markdown
2. **Keycloak Integration** (SIMPLIFIED)
   - Keycloak 26.5.2 running on port 8180
   - Embedded H2 database (data not persistent)
   - Development mode configured
   - Admin console accessible at http://localhost:8180 (admin/admin)
   - Health checks enabled
   - Simple setup for development/testing
```

---

## 12. Update `README.md`

### Update Configuration Section

**Find and UPDATE:**

**REMOVE:**
```env
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```

**REPLACE WITH:**
```env
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

---

## 13. Update `start.ps1` and `start.sh`

### Update the .env file generation in both scripts

**In `start.ps1`, find and UPDATE:**

**REMOVE:**
```powershell
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```

**REPLACE WITH:**
```powershell
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

**In `start.sh`, find and UPDATE:**

**REMOVE:**
```bash
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
```

**REPLACE WITH:**
```bash
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

---

## 14. Delete Obsolete File

**DELETE:**
- `scripts/init-keycloak-db.sql` (no longer needed with H2 embedded database)

---

## Summary of Changes

### Configuration Changes
? Keycloak now uses embedded H2 database  
? Removed PostgreSQL connection configuration  
? Updated Keycloak version: 26.0.7 ? 26.5.2  
? Removed `KEYCLOAK_DB` environment variable  
? Removed `keycloak-data` volume  
? Removed database dependency  

### Documentation Updates
? All references to PostgreSQL backend removed  
? Added warnings about data persistence  
? Updated version numbers throughout  
? Simplified architecture diagrams  
? Updated environment variable examples  

### Files Modified: 14
### Files Deleted: 1

---

## Verification Steps

After making these changes:

1. **Validate docker-compose.yml:**
   ```bash
   docker compose config
   ```

2. **Test startup:**
   ```bash
   docker compose down -v
   docker compose up -d --build
   ```

3. **Verify Keycloak:**
   - Access http://localhost:8180
   - Login with admin/admin
   - Confirm it works

4. **Check data persistence:**
   ```bash
   # Create a test realm in Keycloak
   # Restart container
   docker compose restart keycloak
   # Realm should be gone (H2 is not persistent by default)
   ```

---

## Rollback

If you need to revert to PostgreSQL backend:
```bash
git checkout HEAD -- docker-compose.yml .env
```

Then review the previous configuration in git history.
