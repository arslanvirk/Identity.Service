# CI/CD Setup (Beginner-Friendly)

This guide explains a simple, two?phase CI/CD plan:

- **Phase 1 (Now):** Build Docker images and push to GitHub Container Registry (GHCR)
- **Phase 2 (Later):** Deploy the images to Azure App Service

---

## Phase 1 — CI (Build & Push to GHCR)

### Goal
When you push to `main`, GitHub Actions builds the Docker images and pushes them to GHCR.

### Files Used
- `.github/workflows/ci.yml`
- `docker-compose.yml` (uses profile `ci`)
- `Identity.Service.Web/Dockerfile`

### Step?by?Step

1. **Make sure GHCR is enabled**
   - No custom secrets needed (uses `GITHUB_TOKEN` automatically).

2. **Push code to `main`**
   ```bash
   git add .
   git commit -m "feat: update api"
   git push origin main
   ```

3. **Check GitHub Actions**
   - Go to: `https://github.com/<owner>/<repo>/actions`
   - Look for the **CI/CD Pipeline** workflow

4. **Verify images in GHCR**
   - Go to: `https://github.com/<owner>?tab=packages`

### Images Published

```
ghcr.io/<owner>/identity.service:latest
ghcr.io/<owner>/identity.service:<git-sha>

ghcr.io/<owner>/identity.service.db:latest
ghcr.io/<owner>/identity.service.db:<git-sha>

ghcr.io/<owner>/identity.service.pgadmin:latest
ghcr.io/<owner>/identity.service.pgadmin:<git-sha>
```

---

## Phase 2 — CD (Later: Azure App Service)

### Goal
Deploy the images from GHCR to Azure App Service.

### Not Implemented Yet
This phase will be added later. When you are ready, the steps will be:

1. **Create Azure App Service** (Linux, Docker)
2. **Create Azure Web App for Containers**
3. **Configure GHCR image**
4. **Add GitHub Actions CD workflow**
5. **Store secrets in Azure Key Vault**

---

## Local Development (Optional)

Use the unified compose file with the `dev` profile:

```bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

---

## Quick Summary

- **CI is active now** (build & push to GHCR)
- **CD to Azure is planned** (not configured yet)

When you want to start Azure deployment, I can add a second workflow that deploys the built images.
