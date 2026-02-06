# Production Secrets Setup Script (PowerShell)
# This script creates Docker secrets files for production deployment

$ErrorActionPreference = "Stop"

Write-Host "?? Identity Service - Docker Secrets Setup" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Create secrets directory
$SecretsDir = ".\secrets"
if (Test-Path $SecretsDir) {
    Write-Host "??  Secrets directory already exists. Remove it first? (y/n)" -ForegroundColor Yellow
    $response = Read-Host
    if ($response -eq "y") {
        Remove-Item -Path $SecretsDir -Recurse -Force
        Write-Host "? Removed existing secrets directory" -ForegroundColor Green
    } else {
        Write-Host "? Aborted. Please backup and remove secrets directory manually." -ForegroundColor Red
        exit 1
    }
}

New-Item -ItemType Directory -Path $SecretsDir -Force | Out-Null
Write-Host "? Created secrets directory" -ForegroundColor Green
Write-Host ""

# Helper function to create secret file
function New-Secret {
    param(
        [string]$SecretName,
        [string]$PromptText,
        [string]$DefaultValue = ""
    )
    
    Write-Host "?? $PromptText" -ForegroundColor Yellow
    if ($DefaultValue) {
        Write-Host "   (Press Enter for default: $DefaultValue)" -ForegroundColor Gray
    }
    
    $secureValue = Read-Host -AsSecureString
    $value = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureValue)
    )
    
    if ([string]::IsNullOrWhiteSpace($value) -and $DefaultValue) {
        $value = $DefaultValue
    }
    
    if ([string]::IsNullOrWhiteSpace($value)) {
        Write-Host "? Error: Value cannot be empty" -ForegroundColor Red
        exit 1
    }
    
    $filePath = Join-Path $SecretsDir "$SecretName.txt"
    $value | Out-File -FilePath $filePath -NoNewline -Encoding UTF8
    Write-Host "? Created secret: $SecretName.txt" -ForegroundColor Green
    Write-Host ""
    
    return $value
}

# Generate secure JWT signing key
function New-JwtKey {
    $bytes = New-Object byte[] 32
    $rng = [System.Security.Cryptography.RNGCryptoServiceProvider]::Create()
    $rng.GetBytes($bytes)
    return [Convert]::ToBase64String($bytes)
}

Write-Host "?? JWT Configuration" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan
New-Secret -SecretName "jwt_issuer" -PromptText "Enter JWT Issuer URL:" -DefaultValue "https://api.identityservice.com"
New-Secret -SecretName "jwt_audience" -PromptText "Enter JWT Audience:" -DefaultValue "identity-clients"

Write-Host "?? JWT Signing Key" -ForegroundColor Cyan
Write-Host "   Generate a secure key? (y/n)" -ForegroundColor Yellow
$generateKey = Read-Host
if ($generateKey -eq "y") {
    $jwtKey = New-JwtKey
    $filePath = Join-Path $SecretsDir "jwt_signing_key.txt"
    $jwtKey | Out-File -FilePath $filePath -NoNewline -Encoding UTF8
    Write-Host "? Generated and saved JWT signing key" -ForegroundColor Green
} else {
    New-Secret -SecretName "jwt_signing_key" -PromptText "Enter JWT Signing Key (min 32 chars):"
}
Write-Host ""

Write-Host "???  Database Configuration" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan
$postgresPassword = New-Secret -SecretName "postgres_password" -PromptText "Enter PostgreSQL Password:"

# Build connection string
$postgresUser = "postgres"
$postgresDb = "identitydb"
$connectionString = "Host=db;Port=5432;Database=$postgresDb;Username=$postgresUser;Password=$postgresPassword;Pooling=true;SSL Mode=Require"
$filePath = Join-Path $SecretsDir "connection_string.txt"
$connectionString | Out-File -FilePath $filePath -NoNewline -Encoding UTF8
Write-Host "? Created connection_string.txt" -ForegroundColor Green
Write-Host ""

Write-Host "?? pgAdmin Configuration" -ForegroundColor Cyan
Write-Host "======================="  -ForegroundColor Cyan
New-Secret -SecretName "pgadmin_email" -PromptText "Enter pgAdmin Email:" -DefaultValue "admin@example.com"
New-Secret -SecretName "pgadmin_password" -PromptText "Enter pgAdmin Password:"

Write-Host ""
Write-Host "? All secrets created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Summary:" -ForegroundColor Cyan
Write-Host "===========" -ForegroundColor Cyan
Get-ChildItem -Path $SecretsDir | Format-Table Name, Length, LastWriteTime
Write-Host ""
Write-Host "??  IMPORTANT SECURITY NOTES:" -ForegroundColor Yellow
Write-Host "   1. The .\secrets directory is NOT tracked by git"
Write-Host "   2. Backup secrets securely (use encrypted storage)"
Write-Host "   3. Never commit secrets to version control"
Write-Host "   4. Rotate secrets regularly (every 90 days recommended)"
Write-Host ""
Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host "   1. Deploy with: docker compose -f docker-compose.prod.secrets.yml up -d"
Write-Host "   2. Verify deployment: curl http://localhost:8080/health"
Write-Host "   3. View logs: docker compose -f docker-compose.prod.secrets.yml logs -f"
Write-Host ""
