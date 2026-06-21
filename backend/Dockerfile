# syntax=docker/dockerfile:1

# ===== Stage 1: Build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file project dulu untuk caching layer dependency restore
COPY ["EPMS.sln", "./"]
COPY ["src/EPMS.Domain/EPMS.Domain.csproj", "src/EPMS.Domain/"]
COPY ["src/EPMS.Application/EPMS.Application.csproj", "src/EPMS.Application/"]
COPY ["src/EPMS.Infrastructure/EPMS.Infrastructure.csproj", "src/EPMS.Infrastructure/"]
COPY ["src/EPMS.WebApi/EPMS.WebApi.csproj", "src/EPMS.WebApi/"]
COPY ["tests/EPMS.UnitTests/EPMS.UnitTests.csproj", "tests/EPMS.UnitTests/"]
COPY ["tests/EPMS.IntegrationTests/EPMS.IntegrationTests.csproj", "tests/EPMS.IntegrationTests/"]

RUN dotnet restore "EPMS.sln"

# Copy seluruh source code, lalu build & publish hanya project WebApi
COPY . .
WORKDIR /src/src/EPMS.WebApi
RUN dotnet publish "EPMS.WebApi.csproj" -c Release -o /app/publish --no-restore

# ===== Stage 2: Runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Buat folder uploads (section 4.7, local file storage) dan user non-root
RUN mkdir -p /app/uploads \
    && addgroup --system epms && adduser --system --ingroup epms epms \
    && chown -R epms:epms /app

COPY --from=build /app/publish .

USER epms

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EPMS.WebApi.dll"]
