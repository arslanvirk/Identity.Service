#!/bin/bash
# Quick start script for Identity.Service with Keycloak

echo "?? Starting Identity.Service with Keycloak..."
echo ""

# Check if .env exists
if [ ! -f ".env" ]; then
    echo "??  Warning: .env file not found!"
    echo "Creating .env file with default values..."
    
    cat > .env << 'EOF'
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
EOF
    
    echo "? Created .env file"
    echo ""
fi

# Set profile and start services
export COMPOSE_PROFILES=dev
echo "?? Starting Docker containers..."
docker compose up -d --build

if [ $? -eq 0 ]; then
    echo ""
    echo "? Services started successfully!"
    echo ""
    echo "?? Service URLs:"
    echo "  API:          http://localhost:8080"
    echo "  Swagger:      http://localhost:8080/identity/swagger"
    echo "  Health:       http://localhost:8080/health"
    echo "  Keycloak:     http://localhost:8180 (admin/admin)"
    echo "  pgAdmin:      http://localhost:5050"
    echo ""
    echo "?? View logs:   docker compose logs -f"
    echo "?? Stop:        docker compose down"
    echo ""
else
    echo ""
    echo "? Failed to start services"
    echo "Run 'docker compose logs' to see errors"
    echo ""
    exit 1
fi
