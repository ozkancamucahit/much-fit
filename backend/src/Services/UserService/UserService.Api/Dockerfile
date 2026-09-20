# Step 1: SDK Stage for restoring and building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

# WORKDIR is the repo root inside the container.
# Using /repo means COPY src/... src/... lands at /repo/src/...
# which mirrors the real repo layout and avoids a confusing /src/src/ double-nesting.
WORKDIR /repo

# Disable telemetry & clear fallback folders via environment
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    NUGET_FALLBACK_PACKAGES=""

# Copy root solution configuration files (Central Package Management, build props, NuGet sources)
COPY Directory.Build.props Directory.Build.targets Directory.Packages.props nuget.config ./

# Copy only .csproj files first — layer-cache trick:
# NuGet restore is skipped on rebuild as long as no .csproj changes,
# even if .cs source files changed.
COPY src/Services/UserService/UserService.Api/UserService.Api.csproj           src/Services/UserService/UserService.Api/
COPY src/Shared/Central.Common.Lib/Central.Common.Lib.csproj                   src/Shared/Central.Common.Lib/
COPY src/Shared/Platform.Web/Platform.Web.csproj                               src/Shared/Platform.Web/
COPY src/aspire/Central.ServiceDefaults/Central.ServiceDefaults.csproj         src/aspire/Central.ServiceDefaults/

# Copy full source for all projects needed by UserService.Api
COPY src/Services/UserService/UserService.Api/  src/Services/UserService/UserService.Api/
COPY src/Shared/                                src/Shared/
COPY src/aspire/Central.ServiceDefaults/        src/aspire/Central.ServiceDefaults/

# Restore and publish in one step so both share the same BuildKit cache mount.
# Splitting into two RUN steps causes NETSDK1064 because cache mounts are
# ephemeral per-step overlays — packages from restore are not visible to
# --no-restore in a separate publish step.
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore src/Services/UserService/UserService.Api/UserService.Api.csproj \
        /p:RestoreFallbackFoldersClear=true && \
    dotnet publish src/Services/UserService/UserService.Api/UserService.Api.csproj \
        -c Release \
        -o /app/publish \
        --no-restore \
        /p:RestoreFallbackFoldersClear=true

# Step 2: Runtime Stage — small ASP.NET runtime image, no SDK
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runner

WORKDIR /app

# Copy only the published output from the builder stage (everything else is discarded)
COPY --chown=app:app --from=builder /app/publish .

# Install curl for the runtime health probe. This keeps the image lean while
# allowing Kubernetes / Docker orchestrators to verify the app is healthy.
# Pin the package version to avoid non-deterministic rebuilds and satisfy image linting.
RUN apt-get update && \
    apt-get install -y --no-install-recommends curl=8.5.0-2ubuntu10.13 && \
    rm -rf /var/lib/apt/lists/*

# Port 8080: non-root users cannot bind to ports below 1024
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Health check for the ASP.NET app. Most .NET apps expose /health via the
# health checks middleware, which is the standard controller for orchestration.
HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl --fail --silent http://localhost:8080/health || exit 1

# Switch to the built-in non-root 'app' user (available since .NET 8 aspnet image)
USER app

# Exec form (JSON array) ensures SIGTERM reaches the .NET process directly for graceful shutdown
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
