# Keycloak Simplification - Quick Start

## ?? What's Changing

We're simplifying Keycloak to use **embedded H2 database** instead of PostgreSQL for easier development setup.

### Before (Complex)
- Keycloak 26.0.7 with PostgreSQL backend
- Separate `keycloakdb` database
- Database health checks and dependencies
- Persistent data across restarts

### After (Simple)
- Keycloak 26.5.2 with embedded H2
- No external database needed
- Faster startup
- Data lost on restart (perfect for testing)

---

## ?? Quick Update (Automated)

**Run this script to update everything automatically:**

```powershell
.\update-keycloak-config.ps1
```

This will update:
- ? docker-compose.yml
- ? .env
- ? .github/workflows/ci.yml
- ? README.md
- ? start.ps1
- ? start.sh
- ? Delete obsolete files

---

## ?? Manual Update (If Script Fails)

If the automated script fails, follow: **`UPDATE-INSTRUCTIONS.md`**

It contains step-by-step instructions for all 14 files that need updates.

---

## ? Key Changes

### 1. docker-compose.yml
```yaml
# OLD (PostgreSQL)
keycloak:
  image: quay.io/keycloak/keycloak:26.0.7
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

# NEW (H2 Embedded)
keycloak:
  image: quay.io/keycloak/keycloak:26.5.2
  environment:
    KC_BOOTSTRAP_ADMIN_USERNAME: ${KEYCLOAK_ADMIN:-admin}
    KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-admin}
  command: start-dev
```

### 2. .env
```env
# OLD
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb

# NEW
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
# KEYCLOAK_DB removed
```

### 3. Removed
- `keycloak-data` volume (not needed)
- `scripts/init-keycloak-db.sql` (not needed)
- PostgreSQL dependency
- Health/metrics explicit config

---

## ?? Important Notes

### Data Persistence
**OLD:** Data persisted in PostgreSQL  
**NEW:** Data is **LOST** when container restarts

This is **perfect for:**
- ? Learning Keycloak
- ? Testing authentication flows
- ? Prototyping
- ? Development

This is **NOT good for:**
- ? Long-term testing with specific configuration
- ? Production (obviously)
- ? Demos where you need to preserve state

### Port Binding
**Still on port 8180** - No change here!
- API: http://localhost:8080
- Keycloak: http://localhost:8180

---

## ?? Testing After Update

1. **Validate Configuration:**
   ```bash
   docker compose config
   ```

2. **Clean Start:**
   ```bash
   docker compose down -v
   docker compose up -d --build
   ```

3. **Verify Services:**
   ```bash
   docker compose ps
   # All should show "healthy" or "running"
   ```

4. **Test Keycloak:**
   - Open: http://localhost:8180
   - Login: admin / admin
   - Create a test realm
   - Restart: `docker compose restart keycloak`
   - Verify: Realm should be gone (H2 doesn't persist)

---

## ?? Files Updated

### Core Configuration (3 files)
1. `docker-compose.yml` - Simplified Keycloak service
2. `.env` - Removed KEYCLOAK_DB
3. `.github/workflows/ci.yml` - Updated image version

### Documentation (9 files)
4. `README.md`
5. `DEPLOYMENT.md`
6. `docs/PROJECT-GUIDE.md`
7. `docs/KEYCLOAK-SETUP.md`
8. `docs/KEYCLOAK-INTEGRATION-SUMMARY.md`
9. `docs/QUICK-REFERENCE.md`
10. `docs/ARCHITECTURE-DIAGRAM.md`
11. `SESSION-SUMMARY.md`
12. `CHANGELOG-KEYCLOAK.md`

### Scripts (2 files)
13. `start.ps1`
14. `start.sh`

### Files Deleted (1 file)
15. `scripts/init-keycloak-db.sql`

---

## ?? Rollback

If you need to revert:

```bash
git checkout HEAD -- docker-compose.yml .env .github/workflows/ci.yml
git checkout HEAD -- README.md DEPLOYMENT.md docs/ start.ps1 start.sh
```

---

## ?? When to Use PostgreSQL Backend

If you need persistent data, you can easily switch back:

```yaml
keycloak:
  environment:
    KC_BOOTSTRAP_ADMIN_USERNAME: admin
    KC_BOOTSTRAP_ADMIN_PASSWORD: admin
    KC_DB: postgres
    KC_DB_URL: jdbc:postgresql://db:5432/keycloakdb
    KC_DB_USERNAME: postgres
    KC_DB_PASSWORD: DevPassword123
  depends_on:
    db:
      condition: service_healthy
```

---

## ?? Learning Resources

- **H2 vs PostgreSQL**: See `docs/KEYCLOAK-SETUP.md` for comparison
- **Full instructions**: `UPDATE-INSTRUCTIONS.md`
- **Original simple config**: Your initial request
- **Production setup**: Phase 2 in `docs/CI-CD-SETUP.md`

---

## ? Checklist

- [ ] Run `.\update-keycloak-config.ps1` OR follow `UPDATE-INSTRUCTIONS.md`
- [ ] Validate: `docker compose config`
- [ ] Clean start: `docker compose down -v && docker compose up -d --build`
- [ ] Test Keycloak: http://localhost:8180
- [ ] Verify data is NOT persistent (restart test)
- [ ] Commit changes: `git add . && git commit -m "simplify: use H2 for Keycloak"`
- [ ] Push: `git push origin main`

---

## ?? Help

If you encounter issues:
1. Check `UPDATE-INSTRUCTIONS.md` for detailed steps
2. Review `docker compose logs keycloak`
3. Ensure no files are open/locked in Visual Studio
4. Try manual updates if script fails

---

**Ready to simplify?** Run `.\update-keycloak-config.ps1` now! ??
