# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Set build args (supplied at build time)
ARG GIT_TOKEN
ARG GIT_USER

# Copy csproj files separately for better caching
COPY OfficeService.WorkerService/OfficeService.WorkerService.csproj OfficeService.WorkerService/
COPY OfficeService.Business/OfficeService.Business.csproj OfficeService.Business/
COPY OfficeService.Common/OfficeService.Common.csproj OfficeService.Common/
COPY OfficeService.DAL/OfficeService.DAL.csproj OfficeService.DAL/

# Configure GitHub Packages NuGet source if token provided
RUN if [ ! -z "$GIT_TOKEN" ] && [ ! -z "$GIT_USER" ]; then \
    dotnet nuget add source "https://nuget.pkg.github.com/$GIT_USER/index.json" \
    --name "GitHubPackages" \
    --username "$GIT_USER" \
    --password "$GIT_TOKEN" \
    --store-password-in-clear-text; \
    fi

# Copy packages and log config
COPY OfficeService.WorkerService/appsettings.Production.json ./OfficeService.WorkerService/
COPY OfficeService.WorkerService/nlog.config ./OfficeService.WorkerService/

# Restore dependencies
WORKDIR /src/OfficeService.WorkerService
RUN dotnet restore OfficeService.WorkerService.csproj

# Copy full source and build
COPY OfficeService.WorkerService/ ./OfficeService.WorkerService/
COPY OfficeService.Business/ ./OfficeService.Business/
COPY OfficeService.Common/ ./OfficeService.Common/
COPY OfficeService.DAL/ ./OfficeService.DAL/

# Publish the main service project
RUN dotnet publish OfficeService.WorkerService.csproj -c Release -o /app/publish /p:UseAppHost=false  --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8081

ENTRYPOINT ["dotnet", "OfficeService.WorkerService.dll"]
