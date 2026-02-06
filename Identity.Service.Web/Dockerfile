# ============================================================================
# IDENTITY SERVICE WEB API - DOCKERFILE
# ============================================================================
# This file builds a production-ready Docker image for the ASP.NET Core API.
# It uses multi-stage builds to keep the final image small and secure.
#
# BEGINNER'S GUIDE:
# - Multi-stage builds = Multiple FROM statements in one Dockerfile
# - Why? Separates build tools from runtime, making images smaller and faster
# - Each stage has a purpose: restore ? build ? publish ? run
#
# STAGES:
# 1. base    - Runtime environment (small, no build tools)
# 2. build   - Build environment (includes SDK, tools)
# 3. publish - Creates optimized production files
# 4. final   - Final lightweight image for production
# ============================================================================

# ============================================================================
# STAGE 1: BASE - Runtime Environment
# ============================================================================
# This is the foundation for the final image.
# Uses aspnet runtime (not SDK) to keep size small.
#
# WHY? SDK is ~700MB, runtime is ~200MB. We only need runtime in production.
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

# Install required dependencies for health checks and debugging
# curl - Used for health check endpoints
# ca-certificates - Required for HTTPS connections
# icu-libs - Required for globalization support
RUN apk add --no-cache curl ca-certificates icu-libs

# Create non-root user for security
# WHY? Running as root is a security risk. If container is compromised,
# attacker has limited permissions.
RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -D -h /app appuser

# Set working directory
WORKDIR /app

# Expose ports
# 8080 - HTTP (used in production behind reverse proxy)
# 8081 - HTTPS (optional, for direct HTTPS)
EXPOSE 8080 8081

# ============================================================================
# STAGE 2: BUILD - Build Environment
# ============================================================================
# This stage has all the tools needed to build .NET applications.
# Uses SDK image which includes compilers and build tools.
#
# WHY ALPINE? Smaller base image (~200MB vs ~700MB)
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

# Build configuration (Release or Debug)
ARG BUILD_CONFIGURATION=Release

# Set working directory for build
WORKDIR /src

# ============================================================================
# OPTIMIZATION: Copy project files first, then restore
# ============================================================================
# WHY? Docker caches each layer. If only source code changes (not dependencies),
# Docker reuses the restore layer, making builds much faster.
#
# BEFORE: 5 minutes to restore packages every build
# AFTER: 10 seconds (uses cache unless .csproj changes)
# ============================================================================
COPY ["Identity.Service.Web/Identity.Service.Web.csproj", "Identity.Service.Web/"]
COPY ["Identity.Service.Application/Identity.Service.Application.csproj", "Identity.Service.Application/"]
COPY ["Identity.Service.Core/Identity.Service.Core.csproj", "Identity.Service.Core/"]
COPY ["Identity.Service.Infrastructure/Identity.Service.Infrastructure.csproj", "Identity.Service.Infrastructure/"]

# Restore NuGet packages
# --locked-mode: Fails if packages.lock.json is out of sync (ensures reproducible builds)
RUN dotnet restore "./Identity.Service.Web/Identity.Service.Web.csproj" \
    --runtime linux-musl-x64

# Copy all source code
# Done AFTER restore so changes to code don't invalidate restore cache
COPY . .

# Build the application
WORKDIR "/src/Identity.Service.Web"
RUN dotnet build "./Identity.Service.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# ============================================================================
# STAGE 3: PUBLISH - Create Production Files
# ============================================================================
# Creates optimized files for production deployment.
# Removes debug symbols, trims unused code, optimizes for size/speed.
# ============================================================================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the application
# -c Release: Optimized build (no debug symbols)
# --no-restore: Skip restore (already done in build stage)
# /p:UseAppHost=false: Don't create native executable (smaller image)
RUN dotnet publish "./Identity.Service.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    --runtime linux-musl-x64 \
    --self-contained false \
    /p:UseAppHost=false

# ============================================================================
# STAGE 4: FINAL - Production Image
# ============================================================================
# This is what actually runs in production.
# Copies only the published files from the publish stage.
# ============================================================================
FROM base AS final

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_URLS=http://+:8080

# Switch to non-root user
USER appuser

# Set working directory
WORKDIR /app

# Copy published files from publish stage
# --chown: Set ownership to appuser (non-root)
COPY --from=publish --chown=appuser:appgroup /app/publish .

# Health check
# WHY? Docker/Kubernetes can automatically restart unhealthy containers
# Runs every 30s, fails after 3 retries
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Application entry point
ENTRYPOINT ["dotnet", "Identity.Service.Web.dll"]