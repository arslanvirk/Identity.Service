# Identity.Service

ASP.NET Core Identity-based REST API on .NET 9 using EF Core + PostgreSQL + Keycloak. Features registration, login (JWT), password reset, health checks, and Swagger.

## 🚀 Quickstart

**Docker Compose (local dev):**

```bash
# PowerShell
$env:COMPOSE_PROFILES="dev"
docker compose up -d --build

# bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

**Access services:**
- 🌐 API: `http://localhost:8080`
- 📚 Swagger: `http://localhost:8080/identity/swagger`
- ❤️ Health: `http://localhost:8080/health`
- 🔐 Keycloak: `http://localhost:8180` (admin/admin)
- 🗄️ pgAdmin: `http://localhost:5050`

## 🏗️ Architecture

- **Framework**: .NET 9
- **Database**: PostgreSQL 16
- **Identity Provider**: Keycloak 26.5.2
- **ORM**: Entity Framework Core
- **Authentication**: JWT + Keycloak
- **API Documentation**: Swagger/OpenAPI

## ⚙️ Configuration

Create a `.env` file (see `.env` in repo root):

```env
# JWT Configuration
JWT_ISSUER=http://identity.local
JWT_AUDIENCE=identity-clients
JWT_SIGNING_KEY=dev-signing-key-change-me

# PostgreSQL
POSTGRES_PASSWORD=DevPassword123
POSTGRES_DB=identitydb
POSTGRES_USER=postgres

# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
```

**Note**: For production, use strong passwords and Azure Key Vault for secrets. Keycloak uses embedded H2 database - data is not persistent across restarts.

## 📚 Documentation

- **[Project Guide](docs/PROJECT-GUIDE.md)** - Quick reference
- **[Keycloak Setup](docs/KEYCLOAK-SETUP.md)** - Identity provider configuration
- **[CI/CD Setup](docs/CI-CD-SETUP.md)** - GitHub Actions pipeline
- **[Deployment Guide](DEPLOYMENT.md)** - Local development setup

## 🔄 CI/CD

GitHub Actions automatically:
- ✅ Builds Docker images on push to `main`
- ✅ Pushes to GitHub Container Registry (GHCR)
- ✅ Tags with `:latest` and `:<sha>`

**Images**: API, PostgreSQL, pgAdmin, Keycloak

See [CI/CD Setup Guide](docs/CI-CD-SETUP.md) for details.

## 🛠️ Development

**Prerequisites:**
- Docker Desktop
- .NET 9 SDK (for local development without Docker)

**Run locally:**
```bash
COMPOSE_PROFILES=dev docker compose up -d --build
```

**Stop services:**
```bash
docker compose down
```

**View logs:**
```bash
docker compose logs -f
```

## 🔐 Keycloak Integration

Keycloak provides enterprise-grade identity and access management:
- Single Sign-On (SSO)
- Identity brokering
- User federation
- Admin console

For setup instructions, see [Keycloak Setup Guide](docs/KEYCLOAK-SETUP.md).

## 📦 Project Structure

```
Identity.Service/
├── Identity.Service.Application/  # Application layer (services, DTOs)
├── Identity.Service.Core/         # Domain layer (entities, interfaces)
├── Identity.Service.Infrastructure/ # Infrastructure (EF Core, repositories)
├── Identity.Service.Web/          # API layer (controllers, middleware)
├── docs/                          # Documentation
├── .github/workflows/             # CI/CD pipelines
└── docker-compose.yml             # Container orchestration
```

## 🚀 Next Steps

1. ✅ Run services locally
2. ⬜ Configure Keycloak realm and clients
3. ⬜ Integrate Keycloak with ASP.NET Core
4. ⬜ Test authentication flows
5. ⬜ Deploy to Azure (Phase 2)

## 📄 License

MIT (or your chosen license)
