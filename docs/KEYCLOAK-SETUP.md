# Keycloak Setup Guide

This guide explains how Keycloak is integrated into the Identity Service and how to configure it for local development.

---

## Overview

Keycloak is an open-source identity and access management solution that provides:
- Single Sign-On (SSO)
- Identity brokering and social login
- User federation (LDAP, Active Directory)
- Client adapters
- Admin console and account management

---

## Local Development Setup

### Running Keycloak

Keycloak is automatically started when you run the dev profile:

```bash
# PowerShell
$env:COMPOSE_PROFILES="dev"
docker compose up -d

# bash
COMPOSE_PROFILES=dev docker compose up -d
```

### Accessing Keycloak

- **Admin Console**: http://localhost:8180
- **Username**: `admin` (configured in `.env`)
- **Password**: `admin` (configured in `.env`)

**Note**: Keycloak runs on port **8180** to avoid conflict with the API on port 8080.

---

## Architecture

### Database Configuration

Keycloak uses PostgreSQL as its backend database:
- **Database**: `keycloakdb` (separate from the API's `identitydb`)
- **Host**: `db` (shared PostgreSQL container)
- **Connection**: Configured via `KC_DB_*` environment variables

### Development Mode

Keycloak runs in development mode (`start-dev` command) which:
- ? Disables HTTPS requirement (development only)
- ? Uses simpler configuration
- ? Enables rapid prototyping
- ? **NOT suitable for production**

### Health Checks

Keycloak includes health checks at:
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

Metrics are enabled at `/metrics` for monitoring.

---

## Basic Configuration

### 1. Create a Realm

A realm manages a set of users, credentials, roles, and groups.

1. Log into the admin console
2. Hover over the realm dropdown (top-left)
3. Click **"Create Realm"**
4. Enter realm name (e.g., `identity-service`)
5. Click **Create**

### 2. Create a Client

A client represents an application that wants to use Keycloak.

1. In your realm, go to **Clients** ? **Create client**
2. Configure:
   - **Client ID**: `identity-api`
   - **Client authentication**: ON (for confidential clients)
   - **Valid redirect URIs**: `http://localhost:8080/*`
   - **Web origins**: `http://localhost:8080`
3. Save and note the **Client Secret** from the Credentials tab

### 3. Create Users

1. Go to **Users** ? **Add user**
2. Enter username and other details
3. Click **Create**
4. Go to **Credentials** tab
5. Set password (disable "Temporary" for testing)

### 4. Create Roles

1. Go to **Realm roles** ? **Create role**
2. Enter role name (e.g., `admin`, `user`)
3. Assign roles to users via **Users** ? Select user ? **Role mapping**

---

## Integration with ASP.NET Core

### Installing Keycloak NuGet Packages

```bash
dotnet add package Keycloak.AuthServices.Authentication
dotnet add package Keycloak.AuthServices.Authorization
```

### Configuration (appsettings.json)

```json
{
  "Keycloak": {
    "realm": "identity-service",
    "auth-server-url": "http://localhost:8180/",
    "ssl-required": "none",
    "resource": "identity-api",
    "verify-token-audience": true,
    "credentials": {
      "secret": "your-client-secret"
    },
    "confidential-port": 0
  }
}
```

### Startup Configuration (Program.cs)

```csharp
// Add Keycloak authentication
builder.Services.AddKeycloakAuthentication(builder.Configuration);

// Add Keycloak authorization
builder.Services.AddKeycloakAuthorization(builder.Configuration);

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();
```

---

## Environment Variables

Configure these in your `.env` file:

```env
# Keycloak Admin Credentials
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin

# Keycloak Database
KEYCLOAK_DB=keycloakdb
```

---

## CI/CD Considerations

### Phase 1 (Current)
- Keycloak image is included in the `ci` profile
- Image is pulled from `quay.io/keycloak/keycloak:26.0.7`
- No custom build needed

### Phase 2 (Future - Azure Deployment)
For production deployment, consider:
- Using Azure AD B2C or Azure AD instead
- Or deploying Keycloak with:
  - HTTPS enabled
  - External PostgreSQL (Azure Database for PostgreSQL)
  - Proper secrets management (Azure Key Vault)
  - Clustering for high availability

---

## Troubleshooting

### Keycloak won't start
- Check if PostgreSQL is healthy: `docker compose ps`
- View logs: `docker logs identity-service-keycloak`
- Ensure port 8180 is not in use

### Can't login to admin console
- Verify credentials in `.env` file
- Check `KEYCLOAK_ADMIN` and `KEYCLOAK_ADMIN_PASSWORD`

### Database connection errors
- Verify PostgreSQL is running and healthy
- Check `KEYCLOAK_DB` database exists
- Ensure PostgreSQL credentials match

---

## Resources

- **Official Docs**: https://www.keycloak.org/documentation
- **Admin Console Guide**: https://www.keycloak.org/docs/latest/server_admin/
- **Securing Applications**: https://www.keycloak.org/docs/latest/securing_apps/
- **Keycloak.AuthServices**: https://github.com/NikiforovAll/keycloak-authorization-services-dotnet

---

## Next Steps

1. ? Run Keycloak locally
2. ? Create a realm and client
3. ? Integrate with ASP.NET Core API
4. ? Test authentication flow
5. ? Configure authorization policies
