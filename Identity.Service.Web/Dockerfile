# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Identity.Service.Web/Identity.Service.Web.csproj", "Identity.Service.Web/"]
COPY ["Identity.Service.Application/Identity.Service.Application.csproj", "Identity.Service.Application/"]
COPY ["Identity.Service.Core/Identity.Service.Core.csproj", "Identity.Service.Core/"]
COPY ["Identity.Service.Infrastructure/Identity.Service.Infrastructure.csproj", "Identity.Service.Infrastructure/"]
RUN dotnet restore "./Identity.Service.Web/Identity.Service.Web.csproj"
COPY . .
WORKDIR "/src/Identity.Service.Web"
RUN dotnet build "./Identity.Service.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Identity.Service.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.Service.Web.dll"]