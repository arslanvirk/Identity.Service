# Debugging Guide - Visual Studio & Docker

## ?? Overview

This guide covers debugging the Identity Service API in both Visual Studio and Docker environments.

---

## ?? Quick Start

### Visual Studio (Recommended for Development)
```powershell
# 1. Start dependencies
.\start-visual-studio-debug.ps1

# 2. Open solution in Visual Studio
# 3. Select "Identity.Service (HTTP)" profile
# 4. Press F5
```

### Docker (For Container Testing)
```powershell
# Start with debug profile
$env:COMPOSE_PROFILES="dev"
docker compose -f docker-compose.yml -f docker-compose.debug.yml up -d --build
```

---

## ?? Visual Studio Debugging

### Prerequisites
- Visual Studio 2022
- .NET 9 SDK
- Docker Desktop (for dependencies)

### Setup

#### 1. Start Dependencies Only
```powershell
$env:COMPOSE_PROFILES="dev"
docker compose up -d db keycloak pgadmin
```

#### 2. Open Solution
- File ? Open ? Project/Solution
- Select `Identity.Service.sln`

#### 3. Select Debug Profile
Choose from dropdown:
- **Identity.Service (HTTP)** ? Recommended
- Identity.Service (HTTPS)
- IIS Express

#### 4. Start Debugging
- Press **F5** or click "Start Debugging"
- API starts on http://localhost:8080
- Swagger opens automatically
- Debugger attached

### Debug Features

#### Breakpoints
```csharp
[HttpGet("profile")]
public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
{
    var userId = HttpContext.GetUserId(); // ? Click margin to set breakpoint
    return Ok(await _profileService.GetAsync(userId));
}
```

#### Navigation
- **F5**: Continue
- **F10**: Step Over
- **F11**: Step Into
- **Shift+F11**: Step Out
- **Ctrl+Shift+F10**: Run to Cursor

#### Inspection
- **Locals Window**: View ? Locals (Ctrl+Alt+V, L)
- **Watch Window**: View ? Watch (Ctrl+Alt+W, 1)
- **Immediate Window**: View ? Immediate Window (Ctrl+Alt+I)
- **Call Stack**: View ? Call Stack (Ctrl+Alt+C)

---

## ?? Docker Debugging

### Setup

#### 1. Start Debug Container
```powershell
$env:COMPOSE_PROFILES="dev"
docker compose -f docker-compose.yml -f docker-compose.debug.yml up -d --build
```

#### 2. Attach VS Code Debugger
1. Open VS Code
2. Press `Ctrl+Shift+D`
3. Select ".NET Core Attach to Docker"
4. Press `F5`
5. Select `Identity.Service.Web.dll` process (PID 1)

### Debug Container Features

- **vsdbg**: Remote debugger installed
- **Debug symbols**: Included
- **Logging**: Debug level enabled
- **Source mapping**: Configured in `.vscode/launch.json`

---

## ?? Configuration

### Launch Profiles (launchSettings.json)

```json
{
  "profiles": {
    "Identity.Service (HTTP)": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "ASPNETCORE_URLS": "http://localhost:8080",
        "ConnectionStrings__DefaultConnection": "Host=localhost;Port=5432;...",
        "Keycloak__Authority": "http://localhost:8180/realms/keycloak-demo"
      },
      "applicationUrl": "http://localhost:8080"
    }
  }
}
```

### VS Code Launch Configuration

```json
{
  "name": ".NET Core Attach to Docker",
  "type": "coreclr",
  "request": "attach",
  "processId": "${command:pickRemoteProcess}",
  "pipeTransport": {
    "pipeProgram": "docker",
    "pipeArgs": ["exec", "-i", "identity-service-api-debug"],
    "debuggerPath": "/vsdbg/vsdbg"
  },
  "sourceFileMap": {
    "/src": "${workspaceFolder}"
  }
}
```

---

## ??? Troubleshooting

### Port Already in Use
```powershell
# Stop Docker containers
docker compose down

# Or kill process
netstat -ano | findstr :8080
taskkill /PID <PID> /F
```

### Database Connection Failed
```powershell
# Check database status
docker compose ps db
# Should show "healthy"

# Restart if needed
docker compose up -d db
```

### Keycloak Not Accessible
```powershell
# Check Keycloak status
docker compose ps keycloak

# Test endpoint
curl http://localhost:8180/realms/keycloak-demo/.well-known/openid-configuration
```

### Breakpoints Not Hitting
1. Verify Debug build (not Release)
2. Clean and rebuild solution
3. Check breakpoint is solid red dot
4. Ensure debugger is attached

### Can't Attach to Docker Container
```powershell
# Verify container is running
docker ps | Select-String "identity-service-api-debug"

# Check vsdbg
docker exec identity-service-api-debug ls /vsdbg

# Rebuild if needed
docker compose -f docker-compose.yml -f docker-compose.debug.yml build --no-cache
```

---

## ?? Comparison

| Feature | Visual Studio | Docker |
|---------|---------------|--------|
| **Setup** | Quick | More complex |
| **Speed** | Fast | Slower |
| **Hot Reload** | ? Yes | ? No |
| **Rebuild** | Not needed | Required |
| **Best For** | Daily development | Container testing |
| **Dependencies** | Need Docker for DB/Keycloak | All in Docker |

**Recommendation:** Use Visual Studio for daily development, Docker for deployment testing.

---

## ?? Scripts Available

| Script | Purpose |
|--------|---------|
| **start-visual-studio-debug.ps1** | Start dependencies for VS debugging |
| **start-debug-simple.ps1** | Start Docker debug mode |
| **debug-docker.ps1** | Interactive Docker debug menu |

---

## ?? Service URLs

| Service | URL | Notes |
|---------|-----|-------|
| **API** | http://localhost:8080 | When running |
| **Swagger** | http://localhost:8080/swagger | Auto-opens in VS |
| **Health** | http://localhost:8080/health | - |
| **Keycloak** | http://localhost:8180 | Docker container |
| **Database** | localhost:5432 | Docker container |
| **pgAdmin** | http://localhost:5050 | Docker container |

---

## ? Verification

### Visual Studio
- [ ] Dependencies running: `docker compose ps`
- [ ] Solution opens in Visual Studio
- [ ] Profile selected: "Identity.Service (HTTP)"
- [ ] Press F5 ? API starts ? Swagger opens
- [ ] Breakpoint hits when calling endpoint

### Docker
- [ ] Debug container running: `docker ps`
- [ ] vsdbg installed: `docker exec ... ls /vsdbg`
- [ ] VS Code debugger attaches
- [ ] Breakpoint hits when calling endpoint

---

## ?? Additional Resources

**Files:**
- `launchSettings.json` - VS debug profiles
- `.vscode/launch.json` - VS Code debug config
- `docker-compose.debug.yml` - Docker debug configuration
- `Dockerfile.debug` - Debug container image

**Documentation:**
- [Visual Studio Debugging](https://learn.microsoft.com/en-us/visualstudio/debugger/)
- [VS Code Docker Debugging](https://code.visualstudio.com/docs/containers/debug-common)
- [.NET Core Debugging](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/debugging-overview)

---

## ?? Summary

**For Development:**
```powershell
.\start-visual-studio-debug.ps1
# Open Visual Studio ? Press F5 ? Debug!
```

**For Container Testing:**
```powershell
$env:COMPOSE_PROFILES="dev"
docker compose -f docker-compose.yml -f docker-compose.debug.yml up -d --build
# VS Code ? Ctrl+Shift+D ? F5 ? Debug!
```

Both methods provide full debugging capabilities with breakpoints, variable inspection, and step-through execution.
