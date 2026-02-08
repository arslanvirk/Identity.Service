#!/usr/bin/env pwsh
# Automated Keycloak Configuration Script
# This script will configure Keycloak using the Admin REST API

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "?? Automated Keycloak Configuration" -ForegroundColor Cyan
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

# Configuration
$KEYCLOAK_URL = "http://localhost:8180"
$ADMIN_USERNAME = "admin"
$ADMIN_PASSWORD = "admin"
$REALM_NAME = "keycloak-demo"
$CLIENT_ID = "demo-api"
$TEST_USERNAME = "testuser"
$TEST_PASSWORD = "Test@123"

Write-Host "?? Configuration:" -ForegroundColor Cyan
Write-Host "  Keycloak URL: $KEYCLOAK_URL" -ForegroundColor White
Write-Host "  Realm: $REALM_NAME" -ForegroundColor White
Write-Host "  Client ID: $CLIENT_ID" -ForegroundColor White
Write-Host "  Test User: $TEST_USERNAME" -ForegroundColor White
Write-Host ""

# Check if Keycloak is running
Write-Host "?? Checking if Keycloak is accessible..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "$KEYCLOAK_URL" -Method GET -TimeoutSec 5 -ErrorAction Stop
    Write-Host "? Keycloak is accessible" -ForegroundColor Green
} catch {
    Write-Host "? Cannot connect to Keycloak at $KEYCLOAK_URL" -ForegroundColor Red
    Write-Host "   Make sure Keycloak is running: docker compose ps keycloak" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Function to get admin access token
function Get-AdminToken {
    Write-Host "?? Getting admin access token..." -ForegroundColor Cyan
    
    $tokenUrl = "$KEYCLOAK_URL/realms/master/protocol/openid-connect/token"
    $body = @{
        username      = $ADMIN_USERNAME
        password      = $ADMIN_PASSWORD
        grant_type    = "password"
        client_id     = "admin-cli"
    }
    
    try {
        $response = Invoke-RestMethod -Uri $tokenUrl -Method POST -Body $body -ContentType "application/x-www-form-urlencoded"
        Write-Host "? Admin token obtained" -ForegroundColor Green
        return $response.access_token
    } catch {
        Write-Host "? Failed to get admin token" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
        exit 1
    }
}

# Function to check if realm exists
function Test-RealmExists {
    param($Token)
    
    Write-Host "?? Checking if realm '$REALM_NAME' exists..." -ForegroundColor Cyan
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    try {
        $response = Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME" -Method GET -Headers $headers -ErrorAction Stop
        Write-Host "? Realm '$REALM_NAME' exists" -ForegroundColor Green
        return $true
    } catch {
        if ($_.Exception.Response.StatusCode.Value__ -eq 404) {
            Write-Host "??  Realm '$REALM_NAME' does not exist" -ForegroundColor Yellow
            return $false
        }
        Write-Host "? Error checking realm: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to create realm
function New-Realm {
    param($Token)
    
    Write-Host "?? Creating realm '$REALM_NAME'..." -ForegroundColor Cyan
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    $realmConfig = @{
        realm = $REALM_NAME
        enabled = $true
        displayName = "Demo Realm"
    } | ConvertTo-Json
    
    try {
        Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms" -Method POST -Headers $headers -Body $realmConfig -ErrorAction Stop
        Write-Host "? Realm '$REALM_NAME' created successfully" -ForegroundColor Green
        Start-Sleep -Seconds 2
    } catch {
        Write-Host "? Failed to create realm" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
        exit 1
    }
}

# Function to get client by client ID
function Get-ClientByClientId {
    param($Token, $ClientId)
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    try {
        $clients = Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/clients?clientId=$ClientId" -Method GET -Headers $headers -ErrorAction Stop
        if ($clients.Count -gt 0) {
            return $clients[0]
        }
        return $null
    } catch {
        return $null
    }
}

# Function to create or update client
function Set-Client {
    param($Token)
    
    Write-Host "?? Checking if client '$CLIENT_ID' exists..." -ForegroundColor Cyan
    
    $existingClient = Get-ClientByClientId -Token $Token -ClientId $CLIENT_ID
    
    $clientConfig = @{
        clientId = $CLIENT_ID
        name = "Demo API Client"
        description = "Public client for Swagger UI"
        enabled = $true
        publicClient = $true  # Public client (no secret)
        standardFlowEnabled = $true  # Authorization Code Flow
        directAccessGrantsEnabled = $true  # Direct access grants
        implicitFlowEnabled = $false
        serviceAccountsEnabled = $false
        protocol = "openid-connect"
        rootUrl = "http://localhost:8080"
        baseUrl = "http://localhost:8080"
        redirectUris = @(
            "http://localhost:8080/*",
            "http://localhost:8080/swagger/oauth2-redirect.html"
        )
        webOrigins = @(
            "http://localhost:8080"
        )
        attributes = @{
            "pkce.code.challenge.method" = "S256"
        }
    }
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    if ($existingClient) {
        Write-Host "?? Updating existing client '$CLIENT_ID'..." -ForegroundColor Cyan
        $clientConfig.id = $existingClient.id
        $clientJson = $clientConfig | ConvertTo-Json -Depth 10
        
        try {
            Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/clients/$($existingClient.id)" -Method PUT -Headers $headers -Body $clientJson -ErrorAction Stop
            Write-Host "? Client '$CLIENT_ID' updated successfully" -ForegroundColor Green
            Write-Host "   ? Redirect URIs: http://localhost:8080/* and /swagger/oauth2-redirect.html" -ForegroundColor White
            Write-Host "   ? Public client (no secret required)" -ForegroundColor White
            Write-Host "   ? Standard flow enabled" -ForegroundColor White
            Write-Host "   ? PKCE enabled" -ForegroundColor White
        } catch {
            Write-Host "? Failed to update client" -ForegroundColor Red
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
            exit 1
        }
    } else {
        Write-Host "?? Creating new client '$CLIENT_ID'..." -ForegroundColor Cyan
        $clientJson = $clientConfig | ConvertTo-Json -Depth 10
        
        try {
            Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/clients" -Method POST -Headers $headers -Body $clientJson -ErrorAction Stop
            Write-Host "? Client '$CLIENT_ID' created successfully" -ForegroundColor Green
            Write-Host "   ? Redirect URIs configured" -ForegroundColor White
            Write-Host "   ? Public client (no secret)" -ForegroundColor White
            Write-Host "   ? Standard flow enabled" -ForegroundColor White
        } catch {
            Write-Host "? Failed to create client" -ForegroundColor Red
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
            exit 1
        }
    }
    
    Start-Sleep -Seconds 2
}

# Function to get user by username
function Get-UserByUsername {
    param($Token, $Username)
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    try {
        $users = Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/users?username=$Username&exact=true" -Method GET -Headers $headers -ErrorAction Stop
        if ($users.Count -gt 0) {
            return $users[0]
        }
        return $null
    } catch {
        return $null
    }
}

# Function to create test user
function New-TestUser {
    param($Token)
    
    Write-Host "?? Checking if user '$TEST_USERNAME' exists..." -ForegroundColor Cyan
    
    $existingUser = Get-UserByUsername -Token $Token -Username $TEST_USERNAME
    
    if ($existingUser) {
        Write-Host "??  User '$TEST_USERNAME' already exists" -ForegroundColor Yellow
        $userId = $existingUser.id
    } else {
        Write-Host "?? Creating user '$TEST_USERNAME'..." -ForegroundColor Cyan
        
        $userConfig = @{
            username = $TEST_USERNAME
            email = "testuser@example.com"
            firstName = "Test"
            lastName = "User"
            enabled = $true
            emailVerified = $true
        } | ConvertTo-Json
        
        $headers = @{
            Authorization = "Bearer $Token"
            "Content-Type" = "application/json"
        }
        
        try {
            $response = Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/users" -Method POST -Headers $headers -Body $userConfig -ErrorAction Stop
            Write-Host "? User '$TEST_USERNAME' created successfully" -ForegroundColor Green
            
            # Get the created user ID
            Start-Sleep -Seconds 1
            $newUser = Get-UserByUsername -Token $Token -Username $TEST_USERNAME
            $userId = $newUser.id
        } catch {
            Write-Host "? Failed to create user" -ForegroundColor Red
            Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
            exit 1
        }
    }
    
    # Set password
    Write-Host "?? Setting password for user '$TEST_USERNAME'..." -ForegroundColor Cyan
    
    $passwordConfig = @{
        type = "password"
        value = $TEST_PASSWORD
        temporary = $false
    } | ConvertTo-Json
    
    $headers = @{
        Authorization = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    
    try {
        Invoke-RestMethod -Uri "$KEYCLOAK_URL/admin/realms/$REALM_NAME/users/$userId/reset-password" -Method PUT -Headers $headers -Body $passwordConfig -ErrorAction Stop
        Write-Host "? Password set successfully" -ForegroundColor Green
        Write-Host "   Username: $TEST_USERNAME" -ForegroundColor White
        Write-Host "   Password: $TEST_PASSWORD" -ForegroundColor White
    } catch {
        Write-Host "? Failed to set password" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Main execution
Write-Host "?? Starting automated configuration..." -ForegroundColor Cyan
Write-Host ""

# Step 1: Get admin token
$adminToken = Get-AdminToken
Write-Host ""

# Step 2: Check/Create realm
if (-not (Test-RealmExists -Token $adminToken)) {
    New-Realm -Token $adminToken
}
Write-Host ""

# Step 3: Create/Update client
Set-Client -Token $adminToken
Write-Host ""

# Step 4: Create test user
New-TestUser -Token $adminToken
Write-Host ""

Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host "? Configuration Complete!" -ForegroundColor Green
Write-Host "=" * 80 -ForegroundColor Cyan
Write-Host ""

Write-Host "?? Summary:" -ForegroundColor Cyan
Write-Host "  ? Realm '$REALM_NAME' configured" -ForegroundColor White
Write-Host "  ? Client '$CLIENT_ID' configured (PUBLIC)" -ForegroundColor White
Write-Host "  ? Redirect URIs set:" -ForegroundColor White
Write-Host "     • http://localhost:8080/*" -ForegroundColor Gray
Write-Host "     • http://localhost:8080/swagger/oauth2-redirect.html" -ForegroundColor Gray
Write-Host "  ? Test user '$TEST_USERNAME' ready" -ForegroundColor White
Write-Host ""

Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Restart your API (optional):" -ForegroundColor Yellow
Write-Host "   docker compose restart api" -ForegroundColor Cyan
Write-Host ""

Write-Host "2. Test with Swagger:" -ForegroundColor Yellow
Write-Host "   • Open: " -NoNewline
Write-Host "http://localhost:8080/swagger" -ForegroundColor Green
Write-Host "   • Click 'Authorize' button" -ForegroundColor White
Write-Host "   • Select scopes: openid, profile, email" -ForegroundColor White
Write-Host "   • Click 'Authorize' again" -ForegroundColor White
Write-Host "   • Login with:" -ForegroundColor White
Write-Host "     - Username: " -NoNewline
Write-Host "$TEST_USERNAME" -ForegroundColor Green
Write-Host "     - Password: " -NoNewline
Write-Host "$TEST_PASSWORD" -ForegroundColor Green
Write-Host "   • Should redirect back as 'Authorized' ?" -ForegroundColor White
Write-Host ""

Write-Host "3. View configuration in Keycloak Admin:" -ForegroundColor Yellow
Write-Host "   " -NoNewline
Write-Host "http://localhost:8180" -ForegroundColor Green
Write-Host ""

# Offer to open Swagger
Write-Host "Would you like to open Swagger UI now? (Y/N): " -NoNewline -ForegroundColor Cyan
$response = Read-Host

if ($response -eq 'Y' -or $response -eq 'y') {
    Write-Host ""
    Write-Host "Opening Swagger UI..." -ForegroundColor Green
    Start-Process "http://localhost:8080/swagger"
    Start-Sleep -Seconds 2
    Write-Host "Opening Keycloak Admin..." -ForegroundColor Green
    Start-Process "http://localhost:8180"
}

Write-Host ""
Write-Host "?? All done! Your Keycloak is configured and ready to use!" -ForegroundColor Green
Write-Host ""
