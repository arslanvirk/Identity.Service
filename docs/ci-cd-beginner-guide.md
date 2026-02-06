# ?? CI/CD Pipeline - Complete Beginner's Guide

## What is CI/CD?

**CI/CD** = Continuous Integration / Continuous Deployment

**Simple Explanation:**
- **CI (Continuous Integration)** = Automatically build and test code every time you push to GitHub
- **CD (Continuous Deployment)** = Automatically deploy to production when tests pass

**Without CI/CD:**
```
1. You write code
2. Push to GitHub
3. Manually build Docker image
4. Manually push to registry
5. Manually deploy to server
6. Repeat for every change... ??
```

**With CI/CD:**
```
1. You write code
2. Push to GitHub
3. ? Magic happens automatically! ?
   - Code is built
   - Tests run
   - Docker images created
   - Pushed to registry
4. Done! ??
```

---

## ?? Your CI/CD Pipeline Overview

### Current Setup

```
???????????????????????????????????????????????????????????????????
?                    YOUR WORKFLOW                                 ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  1. You Push Code to GitHub (main branch)                       ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  2. GitHub Actions Triggers (Automatically!)                    ?
?     - Detects new code on main branch                          ?
?     - Starts CI/CD workflow                                    ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  3. Build Phase (GitHub's cloud servers)                        ?
?     ?? Checkout code                                           ?
?     ?? Set up Docker                                           ?
?     ?? Build 3 images:                                         ?
?        - identity.service (API)                                ?
?        - identity.service.db (PostgreSQL)                      ?
?        - identity.service.pgadmin (pgAdmin)                    ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  4. Tag Images                                                   ?
?     - latest (always newest)                                    ?
?     - abc123d (git commit SHA)                                  ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  5. Push to GitHub Container Registry (GHCR)                    ?
?     - Stores your Docker images                                ?
?     - Anyone can pull: docker pull ghcr.io/arslanvirk/...      ?
???????????????????????????????????????????????????????????????????
                              ?
                              ?
???????????????????????????????????????????????????????????????????
?  6. Summary Posted (See results in GitHub Actions tab)         ?
???????????????????????????????????????????????????????????????????
```

---

## ?? Understanding Your CI Workflow

### File Location
`.github/workflows/ci.yml`

### When It Runs

```yaml
on:
  push:
    branches:
      - main           # Every push to main branch
  workflow_dispatch:   # Manual trigger (button in GitHub)
```

**Example:**
```bash
# This triggers CI
git push origin main

# This does NOT trigger CI
git push origin feature-branch
```

---

### Step-by-Step Breakdown

#### **Step 1: Checkout Repository**
```yaml
- name: Checkout repository
  uses: actions/checkout@v4
```

**What it does:**
- Downloads your code to GitHub's server
- Like: `git clone https://github.com/arslanvirk/Identity.Service`

**Why?**
- GitHub Actions needs your code to build it!

**Beginner Tip:**
> This is like GitHub downloading a fresh copy of your project.

---

#### **Step 2: Set up Docker Buildx**
```yaml
- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3
```

**What it does:**
- Installs Docker build tools
- Enables advanced features (multi-platform builds, caching)

**Why?**
- Makes builds faster with smart caching
- Allows building for different CPU types (x86, ARM)

**Beginner Tip:**
> Buildx is like Docker's "turbo mode" - same builds, but faster!

---

#### **Step 3: Login to GitHub Container Registry**
```yaml
- name: Login to GitHub Container Registry
  uses: docker/login-action@v3
  with:
    registry: ghcr.io
    username: ${{ github.actor }}
    password: ${{ secrets.GITHUB_TOKEN }}
```

**What it does:**
- Logs into ghcr.io (GitHub's Docker registry)
- Uses automatic token (no setup needed!)

**Analogy:**
```
Like logging into Docker Hub, but for GitHub:
  Docker Hub = docker.io/username/image
  GitHub     = ghcr.io/username/image
```

**Beginner Tip:**
> `GITHUB_TOKEN` is automatically created - you don't need to set it up!

---

#### **Step 4: Extract Metadata**
```yaml
- name: Extract metadata
  id: meta
  run: |
    echo "sha_short=$(git rev-parse --short HEAD)" >> $GITHUB_OUTPUT
    echo "repo_owner=$(echo ${{ github.repository_owner }} | tr '[:upper:]' '[:lower:]')" >> $GITHUB_OUTPUT
```

**What it does:**
- Gets git commit SHA (like `a1b2c3d`)
- Gets your username in lowercase (arslanvirk)

**Why?**
- SHA = unique version tag for rollback
- Lowercase = ghcr.io requires lowercase names

**Example Output:**
```
sha_short = a1b2c3d
repo_owner = arslanvirk
```

**Beginner Tip:**
> This creates variables used in later steps!

---

#### **Step 5: Build Images with Docker Compose**
```yaml
- name: Build images with Docker Compose
  run: |
    docker compose -f docker-compose.ci.yml build \
      --build-arg BUILD_CONFIGURATION=Release
```

**What it does:**
- Runs: `docker compose build`
- Uses `docker-compose.ci.yml` (CI-specific config)
- Builds in Release mode (optimized, no debug symbols)

**Why docker-compose?**
- Builds all 3 images together
- Ensures consistent versions
- Simpler than 3 separate `docker build` commands

**What gets built:**
```
1. identity.service (your API)
2. identity.service.db (PostgreSQL)
3. identity.service.pgadmin (pgAdmin)
```

**Beginner Tip:**
> `BUILD_CONFIGURATION=Release` makes your app faster and smaller!

---

#### **Step 6: Tag Images**
```yaml
- name: Tag all images
  run: |
    # Tag API images
    docker tag identity-service-api:ci ghcr.io/arslanvirk/identity.service:latest
    docker tag identity-service-api:ci ghcr.io/arslanvirk/identity.service:a1b2c3d
    
    # (Same for db and pgadmin...)
```

**What it does:**
- Gives images two names:
  - `latest` - always points to newest version
  - `a1b2c3d` - specific commit (for rollback)

**Why two tags?**
```
latest:
  ? Easy to use: docker pull ghcr.io/arslanvirk/identity.service:latest
  ?? Can change unexpectedly

Commit SHA (a1b2c3d):
  ? Never changes (same code every time)
  ? Easy rollback to old versions
  ?? Harder to remember
```

**Beginner Tip:**
> Use `latest` for development, `SHA` for production!

---

#### **Step 7: Push to Registry**
```yaml
- name: Push all images to GHCR
  run: |
    docker push ghcr.io/arslanvirk/identity.service:latest
    docker push ghcr.io/arslanvirk/identity.service:a1b2c3d
```

**What it does:**
- Uploads images to GitHub Container Registry
- Makes them available for download anywhere

**How big are these files?**
```
API:     ~185 MB (optimized!)
DB:      ~238 MB (Alpine)
pgAdmin: ~350 MB
Total:   ~773 MB
```

**Where can you pull from?**
- Your computer: `docker pull ghcr.io/arslanvirk/identity.service:latest`
- Azure: Uses same command
- Any cloud provider: Same!

**Beginner Tip:**
> This is like uploading to Google Drive, but for Docker images!

---

#### **Step 8: Generate Summary**
```yaml
- name: Image summary
  run: |
    echo "### ?? Build Summary" >> $GITHUB_STEP_SUMMARY
    echo "? Build completed successfully" >> $GITHUB_STEP_SUMMARY
    echo "- ghcr.io/arslanvirk/identity.service:latest" >> $GITHUB_STEP_SUMMARY
```

**What it does:**
- Creates a nice summary in GitHub Actions UI
- Shows exactly what was built and pushed

**Example Output:**

```markdown
### ?? Build Summary

? Build completed successfully

?? Images pushed to GHCR:

#### API Service
- ghcr.io/arslanvirk/identity.service:latest
- ghcr.io/arslanvirk/identity.service:a1b2c3d

#### PostgreSQL Database
- ghcr.io/arslanvirk/identity.service.db:latest
- ghcr.io/arslanvirk/identity.service.db:a1b2c3d

#### pgAdmin
- ghcr.io/arslanvirk/identity.service.pgadmin:latest
- ghcr.io/arslanvirk/identity.service.pgadmin:a1b2c3d
```

---

## ?? Viewing Your CI/CD Pipeline

### Step 1: Go to GitHub Actions

```
https://github.com/arslanvirk/Identity.Service/actions
```

### Step 2: See Your Workflows

```
??????????????????????????????????????????????????????
?  CI/CD Pipeline                                     ?
?  ?? ? main - Add Docker Secrets support (2m ago)  ?
?  ?? ? main - Fix YAML indentation (5m ago)        ?
?  ?? ? main - Add multi-container CI/CD (10m ago)  ?
??????????????????????????????????????????????????????
```

### Step 3: Click a Workflow to See Details

```
Build Summary:
  ? Checkout repository          (2s)
  ? Set up Docker Buildx          (5s)
  ? Login to GHCR                 (3s)
  ? Extract metadata              (1s)
  ? Pull base images              (45s)
  ? Build images                  (3m 20s)
  ? Tag all images                (2s)
  ? Push all images to GHCR       (1m 30s)
  ? Image summary                 (1s)

Total time: 5 minutes 49 seconds
```

---

## ?? Using Your Built Images

### Pull from Registry

```bash
# Login to GHCR
echo $GITHUB_TOKEN | docker login ghcr.io -u arslanvirk --password-stdin

# Pull latest version
docker pull ghcr.io/arslanvirk/identity.service:latest
docker pull ghcr.io/arslanvirk/identity.service.db:latest
docker pull ghcr.io/arslanvirk/identity.service.pgadmin:latest

# Or pull specific version
docker pull ghcr.io/arslanvirk/identity.service:a1b2c3d
```

### Run Locally

```bash
# Using production images
docker run -d -p 8080:8080 \
  -e "ConnectionStrings__DefaultConnection=Host=db;..." \
  ghcr.io/arslanvirk/identity.service:latest
```

### Use in Docker Compose

```yaml
# docker-compose.prod.yml
services:
  api:
    image: ghcr.io/arslanvirk/identity.service:latest  # From GHCR!
    # No build section needed - just pull and run!
```

---

## ?? Understanding docker-compose.ci.yml

### Why a Separate File?

```yaml
# docker-compose.yml = Local development
# - Builds from source
# - Uses Development environment
# - Includes debugging tools

# docker-compose.ci.yml = CI/CD builds
# - Builds for production
# - Uses Release configuration
# - No local volumes or ports
```

### What's Different?

```yaml
# docker-compose.ci.yml (simplified)
services:
  api:
    build:
      context: .
      dockerfile: Identity.Service.Web/Dockerfile
      args:
        BUILD_CONFIGURATION: Release  # Optimized build
    image: identity-service-api:ci
    networks:
      - ci-net

  db:
    image: postgres:16-alpine  # Just pull, don't build
    networks:
      - ci-net

  pgadmin:
    image: dpage/pgadmin4:8    # Just pull, don't build
    networks:
      - ci-net
```

**Key Points:**
- ? Builds API from source
- ? Pulls PostgreSQL and pgAdmin (official images)
- ? No ports (CI doesn't need to access them)
- ? No volumes (temporary build, no data to save)
- ? No environment variables (not running, just building)

**Beginner Tip:**
> CI only builds images, doesn't run them. So we skip runtime config!

---

## ?? Secrets in CI/CD

### What Secrets Are Used?

```yaml
- name: Login to GitHub Container Registry
  with:
    password: ${{ secrets.GITHUB_TOKEN }}  # ? Automatic!
```

**Automatic Secrets (No setup needed):**
- `GITHUB_TOKEN` - Access to your repository
- `github.actor` - Your username
- `github.repository_owner` - Repository owner

**Custom Secrets (For future CD):**
```
Settings ? Secrets ? Actions ? New repository secret

Examples:
  AZURE_CREDENTIALS       - For Azure deployment
  DOCKERHUB_TOKEN         - For Docker Hub
  DATABASE_PASSWORD       - Production database
```

**Beginner Tip:**
> Secrets are like passwords - store them in GitHub Settings, use them in workflows!

---

## ?? Triggering Your CI/CD Pipeline

### Method 1: Automatic (Push to Main)

```bash
# Make changes
git add .
git commit -m "Update API"
git push origin main  # ? Triggers CI automatically!
```

### Method 2: Manual (Workflow Dispatch)

1. Go to: `https://github.com/arslanvirk/Identity.Service/actions`
2. Click: "CI/CD Pipeline"
3. Click: "Run workflow"
4. Select branch: `main`
5. Click: "Run workflow" button

**Use cases for manual trigger:**
- Rebuild without code changes
- Test CI pipeline
- Rebuild after registry cleanup

---

## ?? Monitoring Your Pipeline

### Check Build Status

```bash
# Via GitHub CLI (if installed)
gh run list --workflow=ci.yml

# Output:
STATUS      NAME           BRANCH  EVENT  ID
? success   CI/CD Pipeline main    push   123456789
? success   CI/CD Pipeline main    push   123456788
```

### View Live Logs

```
GitHub Actions ? CI/CD Pipeline ? Latest run ? Build step

Real-time output:
  Step 1/12 : FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
  ---> Pulling from mcr.microsoft.com/dotnet/aspnet
  ---> Downloaded 52.3 MB
  Step 2/12 : RUN apk add --no-cache curl
  ---> Running in abc123def456
  ...
```

---

## ?? Troubleshooting

### Build Fails: "Login to GHCR" Step

**Error:**
```
Error: denied: permission_denied: write_package
```

**Solution:**
1. Go to: `https://github.com/arslanvirk/Identity.Service/settings`
2. Actions ? General ? Workflow permissions
3. Select: "Read and write permissions"
4. Click: "Save"

---

### Build Fails: "Build images" Step

**Error:**
```
ERROR [build 3/5] RUN dotnet restore
failed to solve: process "/bin/sh -c dotnet restore" did not complete
```

**Common Causes:**
1. ? Invalid .csproj file
2. ? Network issue (NuGet unavailable)
3. ? Package version doesn't exist

**Solution:**
```bash
# Test locally first
docker compose -f docker-compose.ci.yml build

# If it works locally, CI should work too
```

---

### Images Not Appearing in GHCR

**Check:**
1. Go to: `https://github.com/arslanvirk?tab=packages`
2. Should see: `identity.service`, `identity.service.db`, `identity.service.pgadmin`

**If not visible:**
```yaml
# Check permissions in ci.yml:
permissions:
  contents: read
  packages: write  # ? Must have this!
```

---

### Slow Builds (>10 minutes)

**Causes:**
1. ? No caching (rebuilding packages every time)
2. ? Large Docker context (copying too many files)

**Solutions:**

**1. Add .dockerignore:**
```
# .dockerignore
**/bin/
**/obj/
**/.vs/
**/node_modules/
.git/
```

**2. Check Docker cache:**
```yaml
# Already configured in your setup!
- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3  # ? Enables caching
```

---

## ?? Best Practices

### 1. **Use Semantic Commit Messages**

```bash
# Good ?
git commit -m "feat: Add user authentication endpoint"
git commit -m "fix: Resolve database connection timeout"
git commit -m "docs: Update API documentation"

# Bad ?
git commit -m "Update"
git commit -m "Changes"
git commit -m "fix stuff"
```

**Why?**
- Easy to track what changed
- Clear in GitHub Actions history
- Helpful for teammates

---

### 2. **Test Locally Before Pushing**

```bash
# Before pushing to GitHub:
docker compose -f docker-compose.ci.yml build

# If this works, CI will work!
```

---

### 3. **Use Protected Branches**

```
Settings ? Branches ? Add rule

Rules:
  ? Require pull request before merging
  ? Require status checks to pass (CI must succeed)
  ? Require conversation resolution
```

**Why?**
- CI must pass before merging to main
- Prevents broken code in production
- Forces code review

---

### 4. **Monitor Build Times**

```
If builds take >10 minutes:
  ?? Check .dockerignore (exclude large files)
  ?? Verify layer caching works
  ?? Consider reducing dependencies
```

---

## ?? Quick Reference

### CI/CD Workflow File Locations

```
.github/
?? workflows/
?  ?? ci.yml                   # CI/CD pipeline definition
docker-compose.ci.yml          # CI-specific compose file
Identity.Service.Web/
?? Dockerfile                  # Image build instructions
```

### Key Files and Their Purpose

| File | Purpose | Used By |
|------|---------|---------|
| `.github/workflows/ci.yml` | CI/CD automation | GitHub Actions |
| `docker-compose.ci.yml` | Build configuration | CI pipeline |
| `Dockerfile` | Image build instructions | Docker |
| `.dockerignore` | Files to exclude from build | Docker |

### Useful Links

| Resource | URL |
|----------|-----|
| **GitHub Actions** | https://github.com/arslanvirk/Identity.Service/actions |
| **Packages** | https://github.com/arslanvirk?tab=packages |
| **Docker Compose Docs** | https://docs.docker.com/compose/ |
| **GitHub Actions Docs** | https://docs.github.com/en/actions |

---

## ?? Next Steps

1. **Test your CI pipeline:**
```bash
# Make a small change
echo "# Test" >> README.md
git add README.md
git commit -m "test: Trigger CI pipeline"
git push origin main

# Watch it run:
# https://github.com/arslanvirk/Identity.Service/actions
```

2. **Use the built images:**
```bash
docker pull ghcr.io/arslanvirk/identity.service:latest
docker run -d -p 8080:8080 ghcr.io/arslanvirk/identity.service:latest
```

3. **Learn about CD (Continuous Deployment):**
- Next: Deploy to Azure Container Instances
- Use GitHub Actions to automatically deploy
- See [DEPLOYMENT.md](../DEPLOYMENT.md)

---

**Congratulations!** ?? You now have a fully automated CI pipeline that builds and publishes Docker images every time you push code!
