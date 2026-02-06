#!/bin/bash
# Production Secrets Setup Script
# This script creates Docker secrets files for production deployment

set -e

echo "?? Identity Service - Docker Secrets Setup"
echo "=========================================="
echo ""

# Create secrets directory
SECRETS_DIR="./secrets"
if [ -d "$SECRETS_DIR" ]; then
    echo "??  Secrets directory already exists. Remove it first? (y/n)"
    read -r response
    if [ "$response" = "y" ]; then
        rm -rf "$SECRETS_DIR"
        echo "? Removed existing secrets directory"
    else
        echo "? Aborted. Please backup and remove secrets directory manually."
        exit 1
    fi
fi

mkdir -p "$SECRETS_DIR"
chmod 700 "$SECRETS_DIR"
echo "? Created secrets directory with restricted permissions"
echo ""

# Helper function to create secret file
create_secret() {
    local secret_name=$1
    local prompt_text=$2
    local default_value=$3
    
    echo "?? $prompt_text"
    if [ -n "$default_value" ]; then
        echo "   (Press Enter for default: $default_value)"
    fi
    read -r -s value
    
    if [ -z "$value" ] && [ -n "$default_value" ]; then
        value=$default_value
    fi
    
    if [ -z "$value" ]; then
        echo "? Error: Value cannot be empty"
        exit 1
    fi
    
    echo "$value" > "$SECRETS_DIR/${secret_name}.txt"
    chmod 600 "$SECRETS_DIR/${secret_name}.txt"
    echo "? Created secret: ${secret_name}.txt"
    echo ""
}

# Generate secure JWT signing key
generate_jwt_key() {
    openssl rand -base64 32 2>/dev/null || head -c 32 /dev/urandom | base64
}

echo "?? JWT Configuration"
echo "==================="
create_secret "jwt_issuer" "Enter JWT Issuer URL:" "https://api.identityservice.com"
create_secret "jwt_audience" "Enter JWT Audience:" "identity-clients"

echo "?? JWT Signing Key"
echo "   Generate a secure key? (y/n)"
read -r generate_key
if [ "$generate_key" = "y" ]; then
    jwt_key=$(generate_jwt_key)
    echo "$jwt_key" > "$SECRETS_DIR/jwt_signing_key.txt"
    chmod 600 "$SECRETS_DIR/jwt_signing_key.txt"
    echo "? Generated and saved JWT signing key"
else
    create_secret "jwt_signing_key" "Enter JWT Signing Key (min 32 chars):"
fi
echo ""

echo "???  Database Configuration"
echo "========================"
create_secret "postgres_password" "Enter PostgreSQL Password:"

# Build connection string
POSTGRES_USER="postgres"
POSTGRES_DB="identitydb"
POSTGRES_PASSWORD=$(cat "$SECRETS_DIR/postgres_password.txt")
CONNECTION_STRING="Host=db;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Pooling=true;SSL Mode=Require"
echo "$CONNECTION_STRING" > "$SECRETS_DIR/connection_string.txt"
chmod 600 "$SECRETS_DIR/connection_string.txt"
echo "? Created connection_string.txt"
echo ""

echo "?? pgAdmin Configuration"
echo "======================="
create_secret "pgadmin_email" "Enter pgAdmin Email:" "admin@example.com"
create_secret "pgadmin_password" "Enter pgAdmin Password:"

echo ""
echo "? All secrets created successfully!"
echo ""
echo "?? Summary:"
echo "==========="
ls -lh "$SECRETS_DIR"
echo ""
echo "??  IMPORTANT SECURITY NOTES:"
echo "   1. The ./secrets directory is NOT tracked by git"
echo "   2. Backup secrets securely (use encrypted storage)"
echo "   3. Never commit secrets to version control"
echo "   4. Rotate secrets regularly (every 90 days recommended)"
echo ""
echo "?? Next Steps:"
echo "   1. Deploy with: docker compose -f docker-compose.prod.secrets.yml up -d"
echo "   2. Verify deployment: curl http://localhost:8080/health"
echo "   3. View logs: docker compose -f docker-compose.prod.secrets.yml logs -f"
echo ""
