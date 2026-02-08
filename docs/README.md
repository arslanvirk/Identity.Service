# Documentation Index

Welcome to the Identity.Service documentation! This folder contains consolidated guides for Keycloak integration and debugging.

---

## ?? Documentation Structure

### 1. **[KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)** ? START HERE
Complete guide for Keycloak integration following Milan Jovanovi?'s approach:
- Quick start instructions
- Manual configuration steps
- Code examples and configuration
- Testing and troubleshooting
- Production considerations

### 2. **[DEBUGGING.md](DEBUGGING.md)**
Complete debugging guide for both Visual Studio and Docker:
- Visual Studio debugging setup
- Docker container debugging
- VS Code debugging configuration
- Troubleshooting common issues
- Comparison and recommendations

### 3. **[PROJECT-GUIDE.md](PROJECT-GUIDE.md)**
High-level project overview:
- Technology stack
- Architecture overview
- Getting started

### 4. **[ARCHITECTURE-DIAGRAM.md](ARCHITECTURE-DIAGRAM.md)**
Visual architecture diagrams:
- System overview
- Container communication
- Data flow
- CI/CD pipeline

### 5. **[CI-CD-SETUP.md](CI-CD-SETUP.md)**
CI/CD pipeline configuration:
- GitHub Actions setup
- Container registry configuration
- Build and deployment process

### 6. **[QUICK-REFERENCE.md](QUICK-REFERENCE.md)**
Quick reference for daily development:
- Common commands
- Service URLs and credentials
- Quick troubleshooting

---

## ?? Getting Started

### First Time Setup
```powershell
# 1. Start services
$env:COMPOSE_PROFILES="dev"
docker compose up -d --build

# 2. Configure Keycloak (automated)
.\configure-keycloak-auto.ps1

# 3. Open Swagger and test
# http://localhost:8080/swagger
```

### Daily Development
```powershell
# Visual Studio debugging
.\start-visual-studio-debug.ps1
# Then open solution and press F5
```

---

## ?? Reading Order

### For First-Time Setup:
1. **[KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)** - Complete Keycloak setup
2. **[DEBUGGING.md](DEBUGGING.md)** - Setup debugging environment
3. **[QUICK-REFERENCE.md](QUICK-REFERENCE.md)** - Bookmark for daily use

### For Integration Work:
1. **[KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)** - Configuration reference
2. Code sections in the guide for examples

### For DevOps/Deployment:
1. **[ARCHITECTURE-DIAGRAM.md](ARCHITECTURE-DIAGRAM.md)** - Understand the system
2. **[CI-CD-SETUP.md](CI-CD-SETUP.md)** - Pipeline configuration

---

## ?? Quick Links by Task

| Task | Document | Section |
|------|----------|---------|
| **Start services** | [Quick Reference](QUICK-REFERENCE.md) | Start Services |
| **Configure Keycloak** | [Keycloak Implementation](KEYCLOAK-IMPLEMENTATION.md) | Manual Configuration Steps |
| **Debug in Visual Studio** | [Debugging](DEBUGGING.md) | Visual Studio Debugging |
| **Debug in Docker** | [Debugging](DEBUGGING.md) | Docker Debugging |
| **Understand architecture** | [Architecture Diagram](ARCHITECTURE-DIAGRAM.md) | - |
| **Setup CI/CD** | [CI/CD Setup](CI-CD-SETUP.md) | - |
| **Troubleshoot issues** | [Keycloak Implementation](KEYCLOAK-IMPLEMENTATION.md) | Troubleshooting |

---

## ??? Essential Scripts

| Script | Purpose |
|--------|---------|
| **configure-keycloak-auto.ps1** | Automated Keycloak realm, client, and user setup |
| **start-visual-studio-debug.ps1** | Start dependencies for Visual Studio debugging |
| **cleanup-redundant-docs.ps1** | Remove old/redundant documentation files |

---

## ?? Service URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| **API** | http://localhost:8080 | - |
| **Swagger** | http://localhost:8080/swagger | - |
| **Keycloak Admin** | http://localhost:8180 | admin / admin |
| **pgAdmin** | http://localhost:5050 | dev@local.test / admin123 |

---

## ?? Document Details

| Document | Purpose | Audience |
|----------|---------|----------|
| KEYCLOAK-IMPLEMENTATION.md | Complete Keycloak guide | Developers |
| DEBUGGING.md | Debugging setup | Developers |
| PROJECT-GUIDE.md | Project overview | All |
| ARCHITECTURE-DIAGRAM.md | Visual architecture | Architects |
| CI-CD-SETUP.md | Pipeline guide | DevOps |
| QUICK-REFERENCE.md | Daily commands | All |

---

## ?? Documentation Cleanup

This documentation has been consolidated from multiple sources. Redundant files have been removed:

**Consolidated Into:**
- ? **KEYCLOAK-IMPLEMENTATION.md** - Single source for all Keycloak documentation
- ? **DEBUGGING.md** - Single source for all debugging documentation

**Removed (Redundant):**
- Multiple Keycloak guides (KEYCLOAK-MILAN-GUIDE.md, KEYCLOAK-OAUTH-FIX-SUMMARY.md, etc.)
- Multiple debug guides (DEBUG-IS-WORKING.md, DOCKER-DEBUG-GUIDE.md, etc.)
- Multiple summary files
- Redundant scripts

**To clean up old files:**
```powershell
.\cleanup-redundant-docs.ps1
```

---

## ?? Learning Path

### Beginner (New to project)
1. Read main [README.md](../README.md)
2. Follow [KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md) Quick Start
3. Bookmark [QUICK-REFERENCE.md](QUICK-REFERENCE.md)

### Intermediate (Development)
1. Setup debugging: [DEBUGGING.md](DEBUGGING.md)
2. Reference [KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md) for configuration
3. Use [QUICK-REFERENCE.md](QUICK-REFERENCE.md) daily

### Advanced (Architecture/DevOps)
1. Study [ARCHITECTURE-DIAGRAM.md](ARCHITECTURE-DIAGRAM.md)
2. Configure [CI-CD-SETUP.md](CI-CD-SETUP.md)
3. Implement production considerations from [KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)

---

## ?? Root Directory Files

- **[README.md](../README.md)** - Main project README
- **[DEPLOYMENT.md](../DEPLOYMENT.md)** - Deployment guide
- **docker-compose.yml** - Production configuration
- **docker-compose.debug.yml** - Debug configuration

---

## ?? Getting Help

### Common Issues

**Services won't start?**
- Check Docker is running
- See [Quick Reference - Troubleshooting](QUICK-REFERENCE.md)

**Keycloak configuration issues?**
- See [Keycloak Implementation - Troubleshooting](KEYCLOAK-IMPLEMENTATION.md)
- Or run automated script: `.\configure-keycloak-auto.ps1`

**Debugging not working?**
- See [Debugging - Troubleshooting](DEBUGGING.md)

**Need specific configuration?**
- Search in [KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)

---

## ?? External Resources

- **Milan Jovanovi?'s Article**: https://www.milanjovanovic.tech/blog/integrate-keycloak-with-aspnetcore-using-oauth-2
- **Keycloak Docs**: https://www.keycloak.org/documentation
- **ASP.NET Core Auth**: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/

---

## ? Verification Checklist

- [ ] Read [KEYCLOAK-IMPLEMENTATION.md](KEYCLOAK-IMPLEMENTATION.md)
- [ ] Services running: `docker compose ps`
- [ ] Keycloak configured (realm, client, user)
- [ ] Swagger OAuth2 flow works
- [ ] Debugging environment setup
- [ ] Bookmarked [QUICK-REFERENCE.md](QUICK-REFERENCE.md)

---

**Last Updated:** February 2026  
**Documentation Version:** Consolidated  
**Status:** ? Complete

---

**Happy coding!** ??

For daily tasks, keep [QUICK-REFERENCE.md](QUICK-REFERENCE.md) handy!
