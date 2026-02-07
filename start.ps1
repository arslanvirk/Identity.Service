#!/usr/bin/env pwsh
# Quick start script for Identity.Service with Keycloak

Write-Host "?? Starting Identity.Service with Keycloak..." -ForegroundColor Green
Write-Host ""

# Check if .env exists
if (-not (Test-Path ".env")) {
    Write-Host "??  Warning: .env file not found!" -ForegroundColor Yellow
    Write-Host "Creating .env file with default values..." -ForegroundColor Yellow
    
    $envContent = @"
# Local Environment Variables (DO NOT COMMIT)

# JWT Configuration
JWT_ISSUER=http://identity.local
JWT_AUDIENCE=identity-clients
JWT_SIGNING_KEY=dev-signing-key-change-me

# PostgreSQL Configuration
POSTGRES_PASSWORD=DevPassword123
POSTGRES_DB=identitydb
POSTGRES_USER=postgres
CONNECTION_STRING=Host=db;Port=5432;Database=identitydb;Username=postgres;Password=DevPassword123;Pooling=true

# pgAdmin Configuration
PGADMIN_EMAIL=dev@local.test
PGADMIN_PASSWORD=admin123

# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
"@
    
    Set-Content -Path ".env" -Value $envContent
    Write-Host "? Created .env file" -ForegroundColor Green
    Write-Host ""
}

# Set profile and start services
$env:COMPOSE_PROFILES = "dev"
Write-Host "?? Starting Docker containers..." -ForegroundColor Cyan
docker compose up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "? Services started successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Service URLs:" -ForegroundColor Cyan
    Write-Host "  API:          http://localhost:8080" -ForegroundColor White
    Write-Host "  Swagger:      http://localhost:8080/identity/swagger" -ForegroundColor White
    Write-Host "  Health:       http://localhost:8080/health" -ForegroundColor White
    Write-Host "  Keycloak:     http://localhost:8180 (admin/admin)" -ForegroundColor White
    Write-Host "  pgAdmin:      http://localhost:5050" -ForegroundColor White
    Write-Host ""
    Write-Host "?? View logs:   docker compose logs -f" -ForegroundColor Yellow
    Write-Host "?? Stop:        docker compose down" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "? Failed to start services" -ForegroundColor Red
    Write-Host "Run 'docker compose logs' to see errors" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}
