#!/usr/bin/env pwsh
# Quick Start Script for Visual Studio Debugging

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "?? Visual Studio Debug - Quick Start" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

Write-Host "This script prepares your environment for debugging in Visual Studio" -ForegroundColor White
Write-Host ""

# Step 1: Check Docker is running
Write-Host "Step 1: Checking Docker Desktop..." -ForegroundColor Yellow
$dockerRunning = docker version 2>$null
if (-not $dockerRunning) {
    Write-Host "? Docker is not running!" -ForegroundColor Red
    Write-Host "   Please start Docker Desktop and try again." -ForegroundColor Yellow
    exit 1
}
Write-Host "? Docker is running" -ForegroundColor Green
Write-Host ""

# Step 2: Start dependencies
Write-Host "Step 2: Starting database, Keycloak, and pgAdmin..." -ForegroundColor Yellow
Write-Host "   (API will run in Visual Studio)" -ForegroundColor Gray
Write-Host ""

$env:COMPOSE_PROFILES = "dev"

# Stop any existing containers
Write-Host "   Stopping existing containers..." -ForegroundColor Gray
docker compose down 2>$null | Out-Null

# Start only dependencies (not API)
Write-Host "   Starting dependencies..." -ForegroundColor Gray
docker compose up -d db keycloak pgadmin 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to start services!" -ForegroundColor Red
    exit 1
}

Write-Host "? Dependencies started" -ForegroundColor Green
Write-Host ""

# Step 3: Wait for services to be healthy
Write-Host "Step 3: Waiting for services to be healthy..." -ForegroundColor Yellow
Write-Host "   This may take 20-30 seconds..." -ForegroundColor Gray
Write-Host ""

$maxAttempts = 30
$attempt = 0
$dbHealthy = $false
$keycloakHealthy = $false

while ($attempt -lt $maxAttempts -and (-not $dbHealthy -or -not $keycloakHealthy)) {
    Start-Sleep -Seconds 2
    $attempt++
    
    # Check database
    if (-not $dbHealthy) {
        $dbStatus = docker inspect identity-service-db --format='{{.State.Health.Status}}' 2>$null
        if ($dbStatus -eq "healthy") {
            $dbHealthy = $true
            Write-Host "   ? Database is healthy" -ForegroundColor Green
        }
    }
    
    # Check Keycloak (no health check, just check if running)
    if (-not $keycloakHealthy) {
        $keycloakStatus = docker inspect identity-service-keycloak --format='{{.State.Status}}' 2>$null
        if ($keycloakStatus -eq "running") {
            # Wait a bit more for Keycloak to fully start
            if ($attempt -gt 10) {
                $keycloakHealthy = $true
                Write-Host "   ? Keycloak is running" -ForegroundColor Green
            }
        }
    }
    
    if ($attempt % 5 -eq 0) {
        Write-Host "   Still waiting... ($attempt/$maxAttempts)" -ForegroundColor Gray
    }
}

if (-not $dbHealthy -or -not $keycloakHealthy) {
    Write-Host "??  Services may not be fully ready yet" -ForegroundColor Yellow
    Write-Host "   Check status with: docker compose ps" -ForegroundColor Gray
} else {
    Write-Host "? All services are ready!" -ForegroundColor Green
}

Write-Host ""

# Step 4: Display status
Write-Host "Step 4: Service Status" -ForegroundColor Yellow
Write-Host ""
docker compose ps
Write-Host ""

# Step 5: Instructions
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "? Ready for Visual Studio Debugging!" -ForegroundColor Green
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

Write-Host "?? Services Running:" -ForegroundColor Cyan
Write-Host "  ? Database (PostgreSQL): " -NoNewline
Write-Host "localhost:5432" -ForegroundColor Green
Write-Host "  ? Keycloak: " -NoNewline
Write-Host "http://localhost:8180" -ForegroundColor Green
Write-Host "  ? pgAdmin: " -NoNewline
Write-Host "http://localhost:5050" -ForegroundColor Green
Write-Host ""

Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Open Visual Studio 2022" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "File ? Open ? Project/Solution" -ForegroundColor White
Write-Host "   " -NoNewline
Write-Host "Select: Identity.Service.sln" -ForegroundColor White
Write-Host ""

Write-Host "2. Select Debug Profile" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "At the top toolbar, select: " -NoNewline -ForegroundColor White
Write-Host "Identity.Service (HTTP)" -ForegroundColor Green
Write-Host ""

Write-Host "3. Start Debugging" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "Press " -NoNewline -ForegroundColor White
Write-Host "F5" -ForegroundColor Green -BackgroundColor DarkGray
Write-Host "   " -NoNewline
Write-Host "Or click the green " -NoNewline -ForegroundColor White
Write-Host "? Start Debugging" -ForegroundColor Green
Write-Host "   button" -ForegroundColor White
Write-Host ""

Write-Host "4. Set Breakpoints" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "Open: UserController.cs" -ForegroundColor White
Write-Host "   " -NoNewline
Write-Host "Click in the margin to set breakpoints" -ForegroundColor White
Write-Host ""

Write-Host "5. Test in Swagger" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "Visual Studio will open: " -NoNewline -ForegroundColor White
Write-Host "http://localhost:8080/swagger" -ForegroundColor Green
Write-Host "   " -NoNewline
Write-Host "Authorize with Keycloak and test endpoints" -ForegroundColor White
Write-Host ""

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "?? Documentation" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""
Write-Host "Full guide: " -NoNewline
Write-Host "VISUAL-STUDIO-DEBUG-GUIDE.md" -ForegroundColor Green
Write-Host ""

Write-Host "?? Useful Commands:" -ForegroundColor Cyan
Write-Host "  View status:  " -NoNewline
Write-Host "docker compose ps" -ForegroundColor Gray
Write-Host "  View logs:    " -NoNewline
Write-Host "docker logs identity-service-db -f" -ForegroundColor Gray
Write-Host "  Stop all:     " -NoNewline
Write-Host "docker compose down" -ForegroundColor Gray
Write-Host ""

Write-Host "?? Service URLs:" -ForegroundColor Cyan
Write-Host "  API (when running): " -NoNewline
Write-Host "http://localhost:8080" -ForegroundColor Green
Write-Host "  Swagger:            " -NoNewline
Write-Host "http://localhost:8080/swagger" -ForegroundColor Green
Write-Host "  Keycloak Admin:     " -NoNewline
Write-Host "http://localhost:8180" -ForegroundColor Green -NoNewline
Write-Host " (admin/admin)" -ForegroundColor Gray
Write-Host "  pgAdmin:            " -NoNewline
Write-Host "http://localhost:5050" -ForegroundColor Green -NoNewline
Write-Host " (dev@local.test/admin123)" -ForegroundColor Gray
Write-Host ""

Write-Host "??  Note:" -ForegroundColor Yellow
Write-Host "   The API is NOT running yet - you'll start it in Visual Studio!" -ForegroundColor White
Write-Host "   These services provide database and authentication support." -ForegroundColor White
Write-Host ""

Write-Host "?? Ready to debug in Visual Studio!" -ForegroundColor Green
Write-Host ""

# Option to open Visual Studio
Write-Host "Would you like to open the solution in Visual Studio now? (Y/N): " -NoNewline -ForegroundColor Cyan
$response = Read-Host

if ($response -eq 'Y' -or $response -eq 'y') {
    Write-Host ""
    Write-Host "Opening Visual Studio..." -ForegroundColor Green
    
    $solutionPath = Join-Path $PSScriptRoot "Identity.Service.sln"
    
    if (Test-Path $solutionPath) {
        Start-Process $solutionPath
        Write-Host "? Visual Studio should open shortly!" -ForegroundColor Green
    } else {
        Write-Host "? Solution file not found: $solutionPath" -ForegroundColor Red
        Write-Host "   Please open it manually." -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "Remember to:" -ForegroundColor Cyan
    Write-Host "  1. Select 'Identity.Service (HTTP)' profile" -ForegroundColor White
    Write-Host "  2. Press F5 to start debugging" -ForegroundColor White
}

Write-Host ""
Write-Host "Happy debugging! ????" -ForegroundColor Green
Write-Host ""
