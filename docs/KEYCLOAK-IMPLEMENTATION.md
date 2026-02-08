# Keycloak Implementation - Consolidated Documentation

## ?? Overview

This document consolidates all Keycloak integration documentation following Milan Jovanovi?'s approach.

**Reference Article:** https://www.milanjovanovic.tech/blog/integrate-keycloak-with-aspnetcore-using-oauth-2

---

## ?? Implementation Summary

### What Was Implemented

1. **Keycloak Integration**
   - OAuth 2.0 Authorization Code Flow with PKCE
   - JWT Bearer authentication
   - Swagger OAuth2 integration
   - Public client configuration (no client secret)

2. **Configuration**
   - Simplified Keycloak setup (embedded H2 database)
   - Updated from version 26.0.7 to 26.5.2
   - Port 8180 (avoids API port conflict)

3. **Code Changes**
   - `Program.cs`: JWT Bearer and Swagger OAuth2 configuration
   - `appsettings.json`: Keycloak connection settings
   - `docker-compose.yml`: Simplified Keycloak service

---

## ?? Quick Start

### 1. Configure Keycloak (Manual - One Time)

```powershell
# Option 1: Automated (Recommended)
.\configure-keycloak-auto.ps1

# Option 2: Manual Configuration
# See "Manual Configuration Steps" section below
```

### 2. Start Services

```powershell
$env:COMPOSE_PROFILES="dev"
docker compose up -d --build
```

### 3. Test Integration

1. Open Swagger: http://localhost:8080/swagger
2. Click **"Authorize"** button
3. Login with Keycloak: `testuser` / `Test@123`
4. Call protected endpoints ?

---

## ?? Manual Configuration Steps

If you prefer manual setup or automated script fails:

### Step 1: Create Realm
1. Open Keycloak: http://localhost:8180
2. Login: `admin` / `admin`
3. Create realm: `keycloak-demo`

### Step 2: Create Public Client
1. Go to **Clients** ? **Create client**
2. **Client ID**: `demo-api`
3. **Client authentication**: **OFF** (Public client)
4. **Standard flow**: **ON**
5. **Valid redirect URIs**:
   - `http://localhost:8080/*`
   - `http://localhost:8080/swagger/oauth2-redirect.html`
6. **Web origins**: `http://localhost:8080`
7. **Save**

### Step 3: Create Test User
1. Go to **Users** ? **Add user**
2. **Username**: `testuser`
3. **Email**: `testuser@example.com`
4. **Email verified**: **ON**
5. Create user
6. **Credentials** tab ? Set password: `Test@123` (Temporary: **OFF**)

---

## ?? Configuration Files

### appsettings.json
```json
{
  "Keycloak": {
    "Authority": "http://localhost:8180/realms/keycloak-demo",
    "ClientId": "demo-api",
    "ClientSecret": "",
    "Audience": "account",
    "Issuer": "http://localhost:8180/realms/keycloak-demo",
    "MetadataAddress": "http://localhost:8180/realms/keycloak-demo/.well-known/openid-configuration",
    "ValidateAudience": true,
    "ValidateIssuer": true
  }
}
```

### Program.cs - JWT Bearer Configuration
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keycloakMetadataAddress = builder.Environment.IsDevelopment()
        ? builder.Configuration["Keycloak:MetadataAddress"]!
        : builder.Configuration["Keycloak:MetadataAddress"]!.Replace("localhost", "keycloak");
    
    options.MetadataAddress = keycloakMetadataAddress;
    options.Audience = builder.Configuration["Keycloak:Audience"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Keycloak:Issuer"],
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});
```

### Program.cs - Swagger Configuration
```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Identity Service API",
        Version = "v1",
        Description = "Identity Service API with Keycloak Authentication"
    });

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "User profile" },
                    { "email", "Email address" }
                }
            }
        },
        Description = "Keycloak OAuth2 Authorization Code Flow"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "oauth2"
                }
            },
            new[] { "openid", "profile", "email" }
        }
    });
});

// Swagger UI Configuration
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API v1");
    options.OAuthClientId(keycloakClientId);  // Public client
    options.OAuthUsePkce();  // Enable PKCE
    options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});
```

---

## ?? Docker Configuration

### docker-compose.yml
```yaml
keycloak:
  profiles: ["dev"]
  image: quay.io/keycloak/keycloak:26.5.2
  container_name: identity-service-keycloak
  environment:
    KC_BOOTSTRAP_ADMIN_USERNAME: ${KEYCLOAK_ADMIN:-admin}
    KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-admin}
  command: start-dev
  ports:
    - "8180:8080"
  networks:
    - identity-net
  restart: unless-stopped
```

**Note:** Uses embedded H2 database (data not persistent across restarts)

---

## ?? Testing

### Test OAuth2 Flow
1. Start services: `docker compose up -d`
2. Open Swagger: http://localhost:8080/swagger
3. Click "Authorize"
4. Select scopes: `openid`, `profile`, `email`
5. Click "Authorize" again
6. Login: `testuser` / `Test@123`
7. Should redirect back as "Authorized" ?

### Test API with curl
```bash
# Get token
TOKEN=$(curl -s -X POST 'http://localhost:8180/realms/keycloak-demo/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=password' \
  -d 'client_id=demo-api' \
  -d 'username=testuser' \
  -d 'password=Test@123' \
  -d 'scope=openid profile email' | jq -r '.access_token')

# Call API
curl -X GET 'http://localhost:8080/identity/identity/api/v1/user/profile' \
  -H "Authorization: Bearer $TOKEN"
```

---

## ??? Troubleshooting

### Issue: `invalid_redirect_uri`
**Solution:** Add redirect URIs in Keycloak client:
- `http://localhost:8080/*`
- `http://localhost:8080/swagger/oauth2-redirect.html`

### Issue: `invalid_client`
**Solution:** Verify Client ID matches: `demo-api`

### Issue: `unauthorized_client`
**Solution:** Enable "Standard flow" in Keycloak client settings

### Issue: Token validation fails
**Solutions:**
- Check `MetadataAddress` is accessible
- Verify `Issuer` URL matches exactly
- Ensure `Audience` is correct (`account`)

---

## ?? Scripts Available

| Script | Purpose |
|--------|---------|
| **configure-keycloak-auto.ps1** | Automated Keycloak setup via REST API |
| **setup-keycloak-milan.ps1** | Interactive manual setup guide |

---

## ?? Key Concepts

### Public Client
- **No client secret** required
- Suitable for browser-based applications (Swagger)
- Uses **PKCE** (Proof Key for Code Exchange) for security
- **Authorization Code Flow**

### OAuth 2.0 Flow
1. User clicks "Authorize" in Swagger
2. Redirects to Keycloak login
3. User authenticates
4. Keycloak redirects back with authorization code
5. Swagger exchanges code for access token (with PKCE)
6. Access token used to call API

### JWT Token Validation
The API validates:
- ? Signature (token is from trusted source)
- ? Issuer (token from correct Keycloak realm)
- ? Audience (token intended for this API)
- ? Expiration (token is still valid)

---

## ?? Important Notes

### Development Setup
- Keycloak uses **HTTP** (not HTTPS)
- **H2 database** (data not persistent)
- Default admin credentials

### Data Persistence
**Data is NOT saved across container restarts!**
- All realms will be lost
- All clients will be lost
- All users will be lost
- You'll start with a clean slate

**This is expected and acceptable for development.**

### Production Considerations
For production:
- Use **HTTPS** for all Keycloak URLs
- Use **external PostgreSQL** database
- Strong **admin credentials**
- Client secrets in **Azure Key Vault**
- Proper **CORS** configuration
- **Keycloak clustering**
- Implement **refresh token** flow

---

## ?? Service URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| **API** | http://localhost:8080 | - |
| **Swagger** | http://localhost:8080/swagger | - |
| **Health** | http://localhost:8080/health | - |
| **Keycloak Admin** | http://localhost:8180 | admin / admin |
| **Keycloak Realm** | http://localhost:8180/realms/keycloak-demo | - |
| **pgAdmin** | http://localhost:5050 | dev@local.test / admin123 |

---

## ? Verification Checklist

- [ ] Keycloak realm `keycloak-demo` created
- [ ] Client `demo-api` configured (public, no secret)
- [ ] Redirect URIs added
- [ ] Test user `testuser` created with password
- [ ] Services running: `docker compose ps`
- [ ] Swagger OAuth2 flow works
- [ ] Protected endpoints accessible with token
- [ ] No `invalid_redirect_uri` errors in logs

---

## ?? References

**Milan Jovanovi?'s Article:**
https://www.milanjovanovic.tech/blog/integrate-keycloak-with-aspnetcore-using-oauth-2

**Microsoft Documentation:**
- [ASP.NET Core Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/)
- [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication)

**Keycloak Documentation:**
- [Server Administration](https://www.keycloak.org/docs/latest/server_admin/)
- [Securing Applications](https://www.keycloak.org/docs/latest/securing_apps/)

---

## ?? Summary

**Implementation Complete:**
- ? Keycloak 26.5.2 integrated
- ? OAuth 2.0 Authorization Code Flow with PKCE
- ? JWT Bearer authentication configured
- ? Swagger OAuth2 integration working
- ? Public client approach (no secrets)
- ? Automated setup script available
- ? Comprehensive documentation

**To Get Started:**
1. Run `.\configure-keycloak-auto.ps1`
2. Open Swagger
3. Authorize and test!

---

**Last Updated:** February 2026
**Keycloak Version:** 26.5.2
**.NET Version:** 9.0
