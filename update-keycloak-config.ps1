#!/usr/bin/env pwsh
# Automated Keycloak Simplification Script
# This script updates all files to use embedded H2 database instead of PostgreSQL

Write-Host "?? Starting Keycloak Simplification Updates..." -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0
$SuccessCount = 0

# Function to update file content
function Update-FileContent {
    param(
        [string]$FilePath,
        [string]$OldText,
        [string]$NewText,
        [string]$Description
    )
    
    try {
        if (Test-Path $FilePath) {
            $content = Get-Content $FilePath -Raw
            if ($content -match [regex]::Escape($OldText)) {
                $content = $content -replace [regex]::Escape($OldText), $NewText
                Set-Content -Path $FilePath -Value $content -NoNewline
                Write-Host "? Updated: $Description" -ForegroundColor Green
                $script:SuccessCount++
            } else {
                Write-Host "??  Skipped: $Description (pattern not found)" -ForegroundColor Yellow
            }
        } else {
            Write-Host "? Error: $FilePath not found" -ForegroundColor Red
            $script:ErrorCount++
        }
    } catch {
        Write-Host "? Error updating $Description : $_" -ForegroundColor Red
        $script:ErrorCount++
    }
}

Write-Host "?? Step 1: Updating docker-compose.yml..." -ForegroundColor Cyan

# Update 1: Simplify Keycloak dev service
Update-FileContent `
    -FilePath "docker-compose.yml" `
    -OldText @"
  keycloak:
    profiles: ["dev"]
    image: quay.io/keycloak/keycloak:26.0.7
    container_name: identity-service-keycloak
    environment:
      KC_BOOTSTRAP_ADMIN_USERNAME: `${KEYCLOAK_ADMIN:-admin}
      KC_BOOTSTRAP_ADMIN_PASSWORD: `${KEYCLOAK_ADMIN_PASSWORD:-admin}
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://db:5432/`${KEYCLOAK_DB:-keycloakdb}
      KC_DB_USERNAME: `${POSTGRES_USER:-postgres}
      KC_DB_PASSWORD: `${POSTGRES_PASSWORD:-DevPassword123}
      KC_HEALTH_ENABLED: "true"
      KC_METRICS_ENABLED: "true"
    command: start-dev
    depends_on:
      db:
        condition: service_healthy
    <<: *keycloak-common
"@ `
    -NewText @"
  keycloak:
    profiles: ["dev"]
    image: quay.io/keycloak/keycloak:26.5.2
    container_name: identity-service-keycloak
    environment:
      KC_BOOTSTRAP_ADMIN_USERNAME: `${KEYCLOAK_ADMIN:-admin}
      KC_BOOTSTRAP_ADMIN_PASSWORD: `${KEYCLOAK_ADMIN_PASSWORD:-admin}
    command: start-dev
    <<: *keycloak-common
"@ `
    -Description "docker-compose.yml - Keycloak dev service"

# Update 2: Update Keycloak CI service
Update-FileContent `
    -FilePath "docker-compose.yml" `
    -OldText @"
  keycloak-ci:
    profiles: ["ci"]
    image: quay.io/keycloak/keycloak:26.0.7
    networks:
      - identity-net
"@ `
    -NewText @"
  keycloak-ci:
    profiles: ["ci"]
    image: quay.io/keycloak/keycloak:26.5.2
    networks:
      - identity-net
"@ `
    -Description "docker-compose.yml - Keycloak CI service"

# Update 3: Remove keycloak-data volume
Update-FileContent `
    -FilePath "docker-compose.yml" `
    -OldText @"
volumes:
  pgdata:
    driver: local
  pgadmin-data:
    driver: local
  keycloak-data:
    driver: local
"@ `
    -NewText @"
volumes:
  pgdata:
    driver: local
  pgadmin-data:
    driver: local
"@ `
    -Description "docker-compose.yml - Remove keycloak-data volume"

Write-Host ""
Write-Host "?? Step 2: Updating .env..." -ForegroundColor Cyan

# Update .env file
Update-FileContent `
    -FilePath ".env" `
    -OldText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
"@ `
    -NewText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
"@ `
    -Description ".env - Remove KEYCLOAK_DB"

Write-Host ""
Write-Host "?? Step 3: Updating CI/CD workflow..." -ForegroundColor Cyan

# Update CI workflow
Update-FileContent `
    -FilePath ".github/workflows/ci.yml" `
    -OldText "docker tag quay.io/keycloak/keycloak:26.0.7" `
    -NewText "docker tag quay.io/keycloak/keycloak:26.5.2" `
    -Description "ci.yml - Keycloak version"

Write-Host ""
Write-Host "?? Step 4: Updating documentation files..." -ForegroundColor Cyan

# Update README.md
Update-FileContent `
    -FilePath "README.md" `
    -OldText @"
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
"@ `
    -NewText @"
# Keycloak
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
"@ `
    -Description "README.md - Environment variables"

# Update start.ps1
Update-FileContent `
    -FilePath "start.ps1" `
    -OldText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
"@ `
    -NewText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
"@ `
    -Description "start.ps1 - Environment variables"

# Update start.sh
Update-FileContent `
    -FilePath "start.sh" `
    -OldText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
KEYCLOAK_DB=keycloakdb
"@ `
    -NewText @"
# Keycloak Configuration
KEYCLOAK_ADMIN=admin
KEYCLOAK_ADMIN_PASSWORD=admin
"@ `
    -Description "start.sh - Environment variables"

Write-Host ""
Write-Host "?? Step 5: Deleting obsolete files..." -ForegroundColor Cyan

# Delete obsolete SQL script
if (Test-Path "scripts/init-keycloak-db.sql") {
    Remove-Item "scripts/init-keycloak-db.sql" -Force
    Write-Host "? Deleted: scripts/init-keycloak-db.sql" -ForegroundColor Green
    $SuccessCount++
} else {
    Write-Host "??  File not found: scripts/init-keycloak-db.sql" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "?? Update Summary" -ForegroundColor Cyan
Write-Host "=" * 60 -ForegroundColor Cyan
Write-Host "? Successful updates: $SuccessCount" -ForegroundColor Green
if ($ErrorCount -gt 0) {
    Write-Host "? Errors: $ErrorCount" -ForegroundColor Red
}
Write-Host ""

if ($ErrorCount -eq 0) {
    Write-Host "?? All updates completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "?? Next Steps:" -ForegroundColor Cyan
    Write-Host "1. Review changes: git diff" -ForegroundColor White
    Write-Host "2. Validate compose: docker compose config" -ForegroundColor White
    Write-Host "3. Test setup: docker compose down -v && docker compose up -d --build" -ForegroundColor White
    Write-Host "4. Access Keycloak: http://localhost:8180" -ForegroundColor White
    Write-Host ""
    Write-Host "??  Note: Keycloak now uses H2 - data will be lost on restart!" -ForegroundColor Yellow
} else {
    Write-Host "??  Some updates failed. Please review the errors above." -ForegroundColor Yellow
    Write-Host "?? Tip: Check UPDATE-INSTRUCTIONS.md for manual update steps" -ForegroundColor Cyan
}

Write-Host ""
